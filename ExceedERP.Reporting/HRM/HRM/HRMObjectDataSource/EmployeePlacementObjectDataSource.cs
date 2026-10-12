using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePlacementObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePlacementVM> GetEmployeePlacment(string empId, string branchId,
            string parentOrStructureId, string orgStructureId, string businessUnit, string jobTitle,
            DateTime? fromAssignmentDate, DateTime? toAssignmentDate,
            string employeeType, string status,
            bool isPend, string assignmentType, DateTime? nextSalaryChangeFrom, DateTime? nextSalaryChangeTo)
        {
            // 1. Base query with AsNoTracking to avoid EF change-tracker overhead
            var placementsQuery = db.Placements.AsNoTracking().AsQueryable();

            // 2. Apply Filters
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branId))
                placementsQuery = placementsQuery.Where(x => x.BranchId == branId);

            if (!string.IsNullOrEmpty(parentOrStructureId) && int.TryParse(parentOrStructureId, out int parentOrg))
                placementsQuery = placementsQuery.Where(x => x.ParentOrgStructureId == parentOrg);

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgStruct))
                placementsQuery = placementsQuery.Where(x => x.OrgStructureId == orgStruct);

            if (isPend)
                placementsQuery = placementsQuery.Where(x => x.IsPend);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Active")
                    placementsQuery = placementsQuery.Where(x => x.isCurrent);
                else if (status == "InActive")
                    placementsQuery = placementsQuery.Where(x => !x.isCurrent);
            }

            if (!string.IsNullOrEmpty(employeeType))
                placementsQuery = placementsQuery.Where(x => x.EmployementType.ToString() == employeeType);

            if (!string.IsNullOrEmpty(assignmentType))
                placementsQuery = placementsQuery.Where(x => x.AssignmentType.ToString() == assignmentType);

            if (fromAssignmentDate.HasValue)
                placementsQuery = placementsQuery.Where(x => x.AssignmentDate >= fromAssignmentDate.Value);

            if (toAssignmentDate.HasValue)
                placementsQuery = placementsQuery.Where(x => x.AssignmentDate <= toAssignmentDate.Value);

            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
                placementsQuery = placementsQuery.Where(x => x.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
                placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(jobTitle) && int.TryParse(jobTitle, out int lookUpId))
                placementsQuery = placementsQuery.Where(x => x.LookupId == lookUpId);

            if (nextSalaryChangeFrom.HasValue)
                placementsQuery = placementsQuery.Where(x => x.NextSalaryChangeDate.HasValue && x.NextSalaryChangeDate.Value >= nextSalaryChangeFrom.Value);

            if (nextSalaryChangeTo.HasValue)
                placementsQuery = placementsQuery.Where(x => x.NextSalaryChangeDate.HasValue && x.NextSalaryChangeDate.Value <= nextSalaryChangeTo.Value);

            // 3. Materialize filtered placements
            var filteredPlacements = placementsQuery
                .OrderByDescending(x => x.AssignmentDate)
                .ThenByDescending(x => x.EmployeeId)
                .ToList();

            if (!filteredPlacements.Any())
                return new List<EmployeePlacementVM>();

            var employeeIds = filteredPlacements.Select(p => p.EmployeeId).Distinct().ToList();

            // 4. Batch pre-load data into memory with AsNoTracking
            var employeesDict = db.Person_Employee.AsNoTracking()
                .Where(e => employeeIds.Contains(e.EmployeeId))
                .ToDictionary(e => e.EmployeeId);

            var branchDict = db.Branch.AsNoTracking().ToDictionary(b => b.BranchId);
            var unitDict = db.BusinessUnits.AsNoTracking().ToDictionary(u => u.BusinessUnitId);
            var mainStructDict = db.MainCompanyStructures.AsNoTracking().ToDictionary(m => m.OrgStructureId);
            var rankDict = db.Ranks.AsNoTracking().ToDictionary(r => r.RankId);
            var stepDict = db.Steps.AsNoTracking().ToDictionary(s => s.StepId);

            var salariesDict = db.SalariesJobTitles.AsNoTracking()
                .GroupBy(s => new { s.StepId, s.RankId })
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(s => s.Salary).Select(s => s.Salary).ToList()
                );

            // 5. Pre-load all placements for these employees in ONE query and group them in memory
            var allPlacements = db.Placements.AsNoTracking()
                .Where(p => employeeIds.Contains(p.EmployeeId))
                .ToList();

            var currentPlacementLookup = allPlacements
                .Where(p => p.isCurrent)
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.PlacementId).FirstOrDefault());

            var previousPlacementLookup = allPlacements
                .Where(p => !p.isCurrent)
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.AssignmentDate).ThenByDescending(p => p.PlacementId).FirstOrDefault());

            // 6. Precompute Next-Step mapping (O(1) lookup per row instead of LINQ orderby in loop)
            var orderedSteps = stepDict.Values.OrderBy(s => s.StepSequence).ToList();
            var nextStepMap = new Dictionary<int, Step>();
            for (int i = 0; i < orderedSteps.Count; i++)
            {
                var next = orderedSteps.Skip(i + 1).FirstOrDefault(s => s.StepSequence > orderedSteps[i].StepSequence);
                if (next != null)
                    nextStepMap[orderedSteps[i].StepId] = next;
            }

            bool isAmharic = cultureName.Equals("am", StringComparison.OrdinalIgnoreCase) ||
                             cultureName.Equals("am-et", StringComparison.OrdinalIgnoreCase);
            DateTime today = DateTime.Today;

            var employeeVMs = new List<EmployeePlacementVM>(filteredPlacements.Count);

            // 7. Process rows in-memory with zero extra database hits
            foreach (var place in filteredPlacements)
            {
                employeesDict.TryGetValue(place.EmployeeId, out var employee);
                branchDict.TryGetValue(place.BranchId, out var branch);
                unitDict.TryGetValue(place.BusinessUnitId, out var unit);
                mainStructDict.TryGetValue(place.ParentOrgStructureId ?? 0, out var parentStructure);
                mainStructDict.TryGetValue(place.OrgStructureId, out var orgStructure);
                employeesDict.TryGetValue(place.HeadId ?? 0, out var boss);
                rankDict.TryGetValue(place.RankId, out var rank);
                stepDict.TryGetValue(place.StepId, out var step);

                currentPlacementLookup.TryGetValue(place.EmployeeId, out var currentPlacement);
                previousPlacementLookup.TryGetValue(place.EmployeeId, out var prePlacement);

                stepDict.TryGetValue(currentPlacement?.StepId ?? 0, out var currentStep);
                stepDict.TryGetValue(prePlacement?.StepId ?? 0, out var existingStep);
                branchDict.TryGetValue(prePlacement?.BranchId ?? 0, out var fromBranch);

                // Calculate age in-memory
                string age = string.Empty;
                if (employee?.DateOfBirth != null)
                {
                    DateTime birthDate = employee.DateOfBirth;
                    int years = today.Year - birthDate.Year;
                    int months = today.Month - birthDate.Month;
                    int days = today.Day - birthDate.Day;

                    if (days < 0)
                    {
                        months--;
                        days += DateTime.DaysInMonth(today.Year, today.Month == 1 ? 12 : today.Month - 1);
                    }

                    if (months < 0)
                    {
                        years--;
                        months += 12;
                    }

                    age = $"{years} years, {months} months, {days} days";
                }

                var vm = new EmployeePlacementVM
                {
                    EmployeeId = employee?.EmployeeId ?? 0,
                    EmployeeCode = employee?.EmployeeCode,
                    FullName = employee != null
                        ? (isAmharic ? $"{employee.FirstName} {employee.MiddleName} {employee.LastName}"
                                     : $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}")
                        : string.Empty,
                    HireDate = employee != null ? $"{employee.HiredDate:dd-MM-yyyy} ({employee.HiredDateAmharic} e.c)" : null,
                    FromBranch = fromBranch?.Name,
                    Branch = branch?.Name,
                    BussinessUnit = unit?.Name,
                    ParentStructureName = parentStructure?.Name,
                    Departement = orgStructure?.Name,
                    Boss = boss != null
                        ? (isAmharic ? $"{boss.FirstName} {boss.MiddleName} {boss.LastName}"
                                     : $"{boss.FirstNameEnglish} {boss.MiddleNameEnglish} {boss.LastNameEnglish}")
                        : string.Empty,
                    JobTitle = place.JobTitleName,
                    AssignmentDate = place.AssignmentDate,
                    Grade = rank?.RankName,
                    Step = currentStep?.StepName ?? step?.StepName,
                    ExistingScale = existingStep?.StepName,
                    Salary = currentPlacement?.Salary ?? place.Salary,
                    ScaleDate = currentPlacement?.SalarChangeDate?.ToString("dd.MM.yyyy"),
                    NextScaleDate = currentPlacement?.NextSalaryChangeDate?.ToString("dd.MM.yyyy"),
                    AssignmentType = place.AssignmentType.ToString().ToLower(),
                    Age = age
                };

                // Fast Next Scale / Salary calculation using precomputed map
                if (currentStep != null && nextStepMap.TryGetValue(currentStep.StepId, out var nextStep))
                {
                    vm.NextScale = nextStep.StepName;

                    var lookupKey = new { StepId = nextStep.StepId, RankId = place.RankId };
                    if (salariesDict.TryGetValue(lookupKey, out var salaryList))
                    {
                        var targetSalary = currentPlacement?.Salary ?? place.Salary;
                        vm.NextSalary = salaryList.FirstOrDefault(s => s >= targetSalary);
                    }
                    else
                    {
                        vm.NextSalary = 0;
                    }
                }

                employeeVMs.Add(vm);
            }

            return employeeVMs;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
    //public class EmployeePlacementObjectDataSource : IDisposable
    //{
    //    private ExceedDbContext db = new ExceedDbContext();
    //     private readonly string cultureName = GlobalHelper.GetCurrentCulture();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<EmployeePlacementVM> GetEmployeePlacment(string empId, string branchId,
    //        string parentOrStructureId, string orgStructureId, string businessUnit,string jobTitle,
    //        DateTime? fromAssignmentDate, DateTime? toAssignmentDate,
    //        string employeeType, string status,
    //        bool isPend,string assignmentType, DateTime? nextSalaryChangeFrom, DateTime? nextSalaryChangeTo)
    //    {
    //        var placementsQuery = db.Placements.AsQueryable();

    //        // Filters
    //        if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branId))
    //            placementsQuery = placementsQuery.Where(x => x.BranchId == branId);

    //        if (!string.IsNullOrEmpty(parentOrStructureId) && int.TryParse(parentOrStructureId, out int parentOrg))
    //            placementsQuery = placementsQuery.Where(x => x.ParentOrgStructureId == parentOrg);

    //        if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgStruct))
    //            placementsQuery = placementsQuery.Where(x => x.OrgStructureId == orgStruct);

    //        if (isPend)
    //            placementsQuery = placementsQuery.Where(x => x.IsPend);

    //        if (!string.IsNullOrEmpty(status))
    //        {
    //            if (status == "Active")
    //                placementsQuery = placementsQuery.Where(x => x.isCurrent);
    //            else if (status == "InActive")
    //                placementsQuery = placementsQuery.Where(x => !x.isCurrent);
    //        }

    //        if (!string.IsNullOrEmpty(employeeType))
    //            placementsQuery = placementsQuery.Where(x => x.EmployementType.ToString() == employeeType);

    //        if (!string.IsNullOrEmpty(assignmentType))
    //            placementsQuery = placementsQuery.Where(x => x.AssignmentType.ToString() == assignmentType);

    //        if (fromAssignmentDate.HasValue)
    //            placementsQuery = placementsQuery.Where(x => x.AssignmentDate >= fromAssignmentDate.Value);

    //        if (toAssignmentDate.HasValue)
    //            placementsQuery = placementsQuery.Where(x => x.AssignmentDate <= toAssignmentDate.Value);

    //        if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
    //            placementsQuery = placementsQuery.Where(x => x.EmployeeId == employeeId);

    //        if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
    //            placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == businessUnitId);

    //        if (!string.IsNullOrEmpty(jobTitle) && int.TryParse(jobTitle, out int lookUpId))
    //            placementsQuery = placementsQuery.Where(x => x.LookupId == lookUpId);

    //        // Promotion filter
    //        if (nextSalaryChangeFrom.HasValue)
    //        {
    //            placementsQuery = placementsQuery.Where(x =>x.NextSalaryChangeDate.HasValue &&x.NextSalaryChangeDate.Value >= nextSalaryChangeFrom.Value);
    //        }
    //        if (nextSalaryChangeTo.HasValue)
    //        {
    //            placementsQuery = placementsQuery.Where(x => x.NextSalaryChangeDate.HasValue &&x.NextSalaryChangeDate.Value <= nextSalaryChangeTo.Value);
    //        }
    //        // Materialize main placements list
    //        var filteredPlacements = placementsQuery.OrderByDescending(x => x.AssignmentDate).ThenByDescending(x => x.EmployeeId).ToList();

    //        var employeeIds = filteredPlacements.Select(p => p.EmployeeId).Distinct().ToList();

    //        // Preload related data in dictionaries
    //        var employeesDict = db.Person_Employee
    //            .Where(e => employeeIds.Contains(e.EmployeeId))
    //            .ToDictionary(e => e.EmployeeId);

    //        var branchDict = db.Branch.ToDictionary(b => b.BranchId);
    //        var unitDict = db.BusinessUnits.ToDictionary(u => u.BusinessUnitId);
    //        var mainStructDict = db.MainCompanyStructures.ToDictionary(m => m.OrgStructureId);
    //        var rankDict = db.Ranks.ToDictionary(r => r.RankId);
    //        var stepDict = db.Steps.ToDictionary(s => s.StepId);
    //        //var salariesDict = db.SalariesJobTitles
    //        //    .GroupBy(s =>new { s.StepId, s.RankId})
    //        //    .ToDictionary(g => g.Key, g => g.OrderBy(s => s.Salary).ToList());

    //        // Group in database or fetch relevant dictionary using the composite anonymous object key

    //        var salariesDict = db.SalariesJobTitles
    //            .GroupBy(s => new { s.StepId, s.RankId })
    //            .ToDictionary(
    //                g => g.Key,
    //                g => g.OrderBy(s => s.Salary).Select(s => s.Salary).ToList()
    //            );

    //        var allPlacements = db.Placements
    //            .Where(p => employeeIds.Contains(p.EmployeeId))
    //            .ToList();

    //        var employeeVMs = new List<EmployeePlacementVM>();

    //        foreach (var place in filteredPlacements)
    //        {

    //            employeesDict.TryGetValue(place.EmployeeId, out var employee);
    //            branchDict.TryGetValue(place.BranchId, out var branch);
    //            unitDict.TryGetValue(place.BusinessUnitId, out var unit);
    //            mainStructDict.TryGetValue(place.ParentOrgStructureId ?? 0, out var parentStructure);
    //            mainStructDict.TryGetValue(place.OrgStructureId , out var orgStructure);
    //            employeesDict.TryGetValue(place.HeadId ?? 0, out var boss);
    //            rankDict.TryGetValue(place.RankId, out var rank);
    //            stepDict.TryGetValue(place.StepId, out var step);

    //            var currentPlacement = allPlacements
    //                .Where(p => p.EmployeeId == place.EmployeeId && p.isCurrent)
    //                .OrderByDescending(p => p.PlacementId)
    //                .FirstOrDefault();

    //            var existingPlacement = allPlacements
    //                .Where(p => p.EmployeeId == place.EmployeeId && !p.isCurrent)
    //                .OrderByDescending(p => p.PlacementId)
    //                .FirstOrDefault();

    //            stepDict.TryGetValue(currentPlacement?.StepId ?? 0, out var currentStep);
    //            stepDict.TryGetValue(existingPlacement?.StepId ?? 0, out var existingStep);
    //            string age = "";
    //            if (employee != null && employee.DateOfBirth != null)
    //            {
    //                DateTime birthDate = employee.DateOfBirth;
    //                DateTime today = DateTime.Today;

    //                int years = today.Year - birthDate.Year;
    //                int months = today.Month - birthDate.Month;
    //                int days = today.Day - birthDate.Day;

    //                // Adjust if the current day/month is before the birthday
    //                if (days < 0)
    //                {
    //                    months--;
    //                    days += DateTime.DaysInMonth(today.Year, (today.Month == 1 ? 12 : today.Month - 1));
    //                }

    //                if (months < 0)
    //                {
    //                    years--;
    //                    months += 12;
    //                }

    //                // 👇 Store as a string
    //                age = $"{years} years, {months} months, {days} days";
    //            }

    //            var prePlacementsQuery = db.Placements
    //                                            .Where(x => !x.isCurrent&&x.EmployeeId == place.EmployeeId)
    //                                            .OrderByDescending(x => x.AssignmentDate).FirstOrDefault();

    //            branchDict.TryGetValue(prePlacementsQuery?.BranchId??0, out var Frombranch);

    //            var vm = new EmployeePlacementVM
    //            {
    //                EmployeeId = employee?.EmployeeId??0,
    //                EmployeeCode = employee?.EmployeeCode,
    //                FullName = employee != null? (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
    //                                    ? $"{employee.FirstName} {employee.MiddleName} {employee.LastName}"
    //                                    : $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
    //                                : string.Empty,
    //                HireDate = employee != null ? $"{employee.HiredDate:dd-MM-yyyy} ({employee.HiredDateAmharic} e.c)" : null,
    //                FromBranch = Frombranch?.Name,
    //                Branch = branch?.Name,
    //                BussinessUnit = unit?.Name,
    //                ParentStructureName = parentStructure?.Name,
    //                Departement = orgStructure?.Name,
    //                Boss = boss != null ? (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
    //                                    ? $"{boss.FirstName} {boss.MiddleName} {boss.LastName}"
    //                                    : $"{boss.FirstNameEnglish} {boss.MiddleNameEnglish} {boss.LastNameEnglish}": string.Empty,

    //                JobTitle = place.JobTitleName,
    //                AssignmentDate = place.AssignmentDate,
    //                Grade = rank?.RankName,
    //                Step = currentStep?.StepName ?? step?.StepName,
    //                ExistingScale = existingStep?.StepName,
    //                Salary = currentPlacement?.Salary ?? place.Salary,
    //                ScaleDate = currentPlacement?.SalarChangeDate?.ToString("dd.MM.yyyy"),
    //                NextScaleDate = currentPlacement?.NextSalaryChangeDate != null
    //                    ? currentPlacement.NextSalaryChangeDate.Value.ToString("dd.MM.yyyy"): null,

    //                AssignmentType = place.AssignmentType.ToString().ToLower(),
    //                Age = age
    //            };

    //            // Next scale and salary
    //            if (currentStep != null)
    //            {
    //                var nextStep = stepDict.Values
    //                    .Where(s => s.StepSequence > currentStep.StepSequence)
    //                    .OrderBy(s => s.StepSequence)
    //                    .FirstOrDefault();

    //                if (nextStep != null)
    //                {
    //                    vm.NextScale = nextStep.StepName;

    //                    //  Create matching composite key for dictionary lookup
    //                    var lookupKey = new { StepId = nextStep.StepId, RankId = place.RankId };

    //                    if (salariesDict.TryGetValue(lookupKey, out var salaryList))
    //                    {
    //                        var targetSalary = currentPlacement?.Salary ?? place.Salary;

    //                        //  Find the next salary step that meets or exceeds current salary
    //                        vm.NextSalary = salaryList.FirstOrDefault(s => s >= targetSalary);
    //                    }
    //                    else
    //                    {
    //                        vm.NextSalary = 0;
    //                    }
    //                }
    //            }

    //            employeeVMs.Add(vm);
    //        }

    //        return employeeVMs;
    //    }

    //    //public List<EmployeePlacementVM> GetEmployeePlacment( string empId, string branchId,
    //    //                                                        string parentOrStructureId, string orgStructureId,
    //    //                                                         DateTime? fromAssignmentDate, DateTime? toAssignmentDate,
    //    //                                                         string employeeType, string status,
    //    //                                                         bool isPend, string promotionYear, string month)
    //    //{
    //    //    var placementsQuery = db.Placements.AsQueryable();

    //    //    if (int.TryParse(branchId, out int branId))
    //    //        placementsQuery = placementsQuery.Where(x => x.BranchId == branId);

    //    //    if (int.TryParse(parentOrStructureId, out int parentOrg))
    //    //        placementsQuery = placementsQuery.Where(x => x.ParentOrgStructureId == parentOrg);

    //    //    if (int.TryParse(orgStructureId, out int orgStruct))
    //    //        placementsQuery = placementsQuery.Where(x => x.OrgStructureId == orgStruct);

    //    //    if (isPend)
    //    //        placementsQuery = placementsQuery.Where(x => x.IsPend);

    //    //    if (!string.IsNullOrEmpty(status))
    //    //    {
    //    //        if (status == "Active")
    //    //            placementsQuery = placementsQuery.Where(x => x.isCurrent);
    //    //        else if (status == "InActive")
    //    //            placementsQuery = placementsQuery.Where(x => !x.isCurrent);
    //    //    }

    //    //    if (!string.IsNullOrEmpty(employeeType))
    //    //        placementsQuery = placementsQuery.Where(x => x.EmployementType.ToString() == employeeType);

    //    //    if (fromAssignmentDate.HasValue)
    //    //        placementsQuery = placementsQuery.Where(x => x.AssignmentDate.Date >= fromAssignmentDate.Value.Date);

    //    //    if (toAssignmentDate.HasValue)
    //    //        placementsQuery = placementsQuery.Where(x => x.AssignmentDate.Date <= toAssignmentDate.Value.Date);

    //    //    if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
    //    //        placementsQuery = placementsQuery.Where(x => x.EmployeeId == employeeId);

    //    //    // Promotion filter
    //    //    int totalMonths = 0;
    //    //    if (decimal.TryParse(promotionYear, out decimal proYear))
    //    //        totalMonths += (int)(proYear * 12);

    //    //    if (int.TryParse(month, out int proMonth))
    //    //        totalMonths += proMonth;

    //    //    if (totalMonths > 0)
    //    //    {
    //    //        var dateLimit = DateTime.Now.AddMonths(-totalMonths);
    //    //        var oneMothAHead = dateLimit.AddMonths(-1);
    //    //        placementsQuery = placementsQuery
    //    //            .Where(x => x.SalarChangeDate.HasValue
    //    //            &&( x.SalarChangeDate.Value<=dateLimit ||x.SalarChangeDate.Value<= oneMothAHead));

    //    //        //var promotableIds = CheckToBePromotedEmployeesInMonths(totalMonths);
    //    //        //if (promotableIds.Any())
    //    //        //    placementsQuery = placementsQuery.Where(x => promotableIds.Contains(x.EmployeeId));
    //    //    }

    //    //    var filteredPlacements = placementsQuery
    //    //        .OrderByDescending(x => x.AssignmentDate)
    //    //        .ThenByDescending(x => x.EmployeeId)
    //    //        .ToList();

    //    //    var employeeIds = filteredPlacements.Select(p => p.EmployeeId).Distinct().ToList();
    //    //    var allPlacements = db.Placements.Where(p => employeeIds.Contains(p.EmployeeId)).ToList();

    //    //    var employeeVMs = new List<EmployeePlacementVM>();

    //    //    foreach (var place in filteredPlacements)
    //    //    {
    //    //        var employee = db.Person_Employee.Find(place.EmployeeId);
    //    //        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
    //    //        var unit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == place.BusinessUnitId);
    //    //        var parentStructure = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == place.ParentOrgStructureId);
    //    //        var orgStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //    //        var boss = db.Person_Employee.Find(place.HeadId);
    //    //        var rank = db.Ranks.FirstOrDefault(x => x.RankId == place.RankId);
    //    //        var step = db.Steps.FirstOrDefault(x => x.StepId == place.StepId);

    //    //        var currentPlacements = allPlacements
    //    //            .Where(p => p.EmployeeId == place.EmployeeId && p.isCurrent)
    //    //            .OrderByDescending(p => p.PlacementId)
    //    //            .ToList();

    //    //        var existingPlacements = allPlacements
    //    //            .Where(p => p.EmployeeId == place.EmployeeId && !p.isCurrent)
    //    //            .OrderByDescending(p => p.PlacementId)
    //    //            .ToList();

    //    //        var current = currentPlacements.FirstOrDefault();
    //    //        var existing = existingPlacements.FirstOrDefault();

    //    //        var currentStep = current != null ? db.Steps.Find(current.StepId) : null;
    //    //        var existingStep = existing != null ? db.Steps.Find(existing.StepId) : null;

    //    //        var vm = new EmployeePlacementVM
    //    //        {
    //    //            EmployeeId = place.EmployeeId,
    //    //            EmployeeCode = place.EmployeeCode,
    //    //            FullName = employee != null ? $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}" : null,
    //    //            HireDate = employee != null ? employee.HiredDate.ToString("dd-MM-yyyy") + " (" + employee.HiredDateAmharic + ")" : null,
    //    //            Branch = branch?.Name,
    //    //            BussinessUnit = unit?.Name,
    //    //            ParentStructureName = parentStructure?.Name,
    //    //            Departement = orgStructure?.Name,
    //    //            Boss = boss != null ? $"{boss.EmployeeCode}-{boss.FirstNameEnglish} {boss.MiddleNameEnglish} {boss.LastNameEnglish}" : place.Boss,
    //    //            JobTitle = place.JobTitleName,
    //    //            AssignmentDate = place.AssignmentDate,
    //    //            Grade = rank?.RankName,
    //    //            Step = currentStep?.StepName ?? step?.StepName,
    //    //            ExistingScale = existingStep?.StepName,
    //    //            Salary = current?.Salary ?? place.Salary,
    //    //            ScaleDate = current?.SalarChangeDate?.ToString("dd.MM.yyyy"),
    //    //            NextScaleDate = current?.SalarChangeDate != null? current.SalarChangeDate.Value.AddYears(2).ToString("dd.MM.yyyy") : null,
    //    //            AssignmentType = place.AssignmentType.ToString()
    //    //        };

    //    //        if (currentStep != null)
    //    //        {
    //    //            var nextStep = db.Steps
    //    //                .Where(x => x.StepSequence > currentStep.StepSequence)
    //    //                .OrderBy(x => x.StepSequence)
    //    //                .FirstOrDefault();

    //    //            if (nextStep != null)
    //    //            {
    //    //                vm.NextScale = nextStep.StepName;
    //    //                var nextSalary = db.SalariesJobTitles
    //    //                    .Where(x => x.StepId == nextStep.StepId && x.Salary >= current.Salary)
    //    //                    .OrderBy(x => x.Salary)
    //    //                    .FirstOrDefault();
    //    //                if (nextSalary != null)
    //    //                    vm.NextSalary = nextSalary.Salary;
    //    //            }
    //    //        }

    //    //        employeeVMs.Add(vm);
    //    //    }

    //    //    return employeeVMs;
    //    //}

    //    private IList<int> CheckToBePromotedEmployeesInMonths(int month)
    //    {
    //        var dateLimit = DateTime.Now.AddMonths(-month);

    //        // Get employees with current placements
    //        var currentPlacements = db.Placements
    //            .Where(p => p.isCurrent)
    //            .Select(p => new
    //            {
    //                p.EmployeeId,
    //                p.StepId,
    //                p.RankId
    //            })
    //            .ToList();

    //        var eligibleEmployeeIds = new List<int>();

    //        foreach (var cp in currentPlacements)
    //        {
    //            var hasChanged = db.Placements
    //                .Where(p => p.EmployeeId == cp.EmployeeId && p.AssignmentDate >= dateLimit)
    //                .Any(p => p.StepId != cp.StepId); // Add RankId != cp.RankId if needed

    //            if (!hasChanged)
    //            {
    //                eligibleEmployeeIds.Add(cp.EmployeeId);
    //            }
    //        }

    //        return eligibleEmployeeIds;
    //    }
    //    //private IList<int> CheckToBePromotedEmployees(decimal year)
    //    //{
    //    //    var twoYearsAgo = DateTime.Now.AddYears(-year);

    //    //    var eligibleEmployeeIds = db.Placements
    //    //        .Where(p => p.isCurrent) // only currently working employees
    //    //        .Select(p => p.EmployeeId)
    //    //        .Distinct()
    //    //        .ToList()
    //    //        .Where(empId =>
    //    //        {
    //    //            var placements = db.Placements
    //    //                .Where(p => p.EmployeeId == empId)
    //    //                .OrderByDescending(p => p.AssignmentDate)
    //    //                .ToList();

    //    //            // Get the latest current rank/step
    //    //            var current = placements.FirstOrDefault(p => p.isCurrent);
    //    //            if (current == null) return false;

    //    //            // Check if the employee had the same step for the last 2 years
    //    //            return placements
    //    //                        .Where(p => p.AssignmentDate >= twoYearsAgo)
    //    //                        .All(p => p.StepId == current.StepId);
    //    //        })
    //    //        .ToList();

    //    //    return eligibleEmployeeIds;
    //    //}
    //    public void Dispose()
    //    {
    //        db.Dispose();
    //    }
    //}
}