using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerformanceObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId, string year, string quarterId,string status, string BusinessUnit, string rankName, string empId)
        {
            var result = new List<EmployeePerfromanceVM>();
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(quarterId))
                return result;

            int.TryParse(year, out int yearId);
            int.TryParse(quarterId, out int quarterIdInt);
            int.TryParse(branchId, out int branchIdInt);
            int.TryParse(orgStructureId, out int orgIdInt);
            int.TryParse(BusinessUnit, out int businessUnitId);
            int.TryParse(empId, out int employeeId);

            //Get Fiscal Year
            var fiscalYearIds = db.QuarterLengths
                .Where(x => x.QuarterLengthId == yearId)
                .Select(x => x.GlFiscalYearId)
                .ToList();

            //Base Query
            var query = db.EmployeePerformances
                .Where(x => x.IsSecondLevelApproved);

            if (fiscalYearIds.Any())
                query = query.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));

            if (quarterIdInt > 0)
                query = query.Where(x => x.GlPeriodId == quarterIdInt);

            if (branchIdInt > 0)
                query = query.Where(x => x.BranchId == branchIdInt);

            if (orgIdInt > 0)
                query = query.Where(x => x.OrgStructureId == orgIdInt);

            if (businessUnitId > 0)
                query = query.Where(x => x.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.ResponsibilityLevel.ToString() == status);

            if (employeeId > 0)
                query = query.Where(x => x.EmployeeId == employeeId);

            var performances = query.ToList();
            if (!performances.Any())
                return result;

            // 3️⃣ Preload Required Data
            var employeeIds = performances.Select(x => x.EmployeeId).Distinct().ToList();
            var orgIds = performances.Select(x => x.OrgStructureId).Distinct().ToList();
            var titleIds = performances.Select(x => x.JobTitleId).Distinct().ToList();
            var periodIds = performances.Select(x => x.GlPeriodId).Distinct().ToList();
            var perfIds = performances.Select(x => x.EmployeePerformanceId).ToList();

            var persons = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            var departments = db.MainCompanyStructures
                .Where(x => orgIds.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            var titles = db.Lookups
                .Where(x => titleIds.Contains(x.LookupId))
                .ToDictionary(x => x.LookupId, x => x.Description);

            var quarters = db.Quarters
                .Where(x => periodIds.Contains(x.QuarterId))
                .ToDictionary(x => x.QuarterId, x => x.Name);

            var perfRanks = db.PerformanceRank.ToList();

            // Measurements
            var allMeasurements = db.EmployeeMeasurement
                .Where(x => perfIds.Contains(x.EmployeePerformanceId))
                .ToList();

            var measurementIds = allMeasurements.Select(x => x.EmpMeasurementId).ToList();

            var allScores = db.EmployeePerformanceScores
                .Where(x => measurementIds.Contains(x.EmployeeMeasurementId))
                .ToList();

            // Optimize lookups
            var scoresByMeasurement = allScores
                .GroupBy(x => x.EmployeeMeasurementId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var measurementsByPerformance = allMeasurements
                .GroupBy(x => x.EmployeePerformanceId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var criteriaLookup = db.MeasurementDefinitions
                .ToDictionary(x => x.MeasurementDefinitionId);

            var measurementLookup = db.Measurements
                .ToDictionary(x => x.MeasurementId);

            var actionLookup = db.PerformanceActions
                .ToDictionary(x => x.ActionId);
            var quarterLengthLookup = db.QuarterLengths
                .ToDictionary(x => x.GlFiscalYearId);
            var quarterLookup = db.Quarters
                .ToDictionary(x => x.QuarterId);

            //  Group by Employee
            var groupedByEmployee = performances.GroupBy(x => x.EmployeeId);

            foreach (var employeeGroup in groupedByEmployee)
            {
                var first = employeeGroup.First();

                var vm = new EmployeePerfromanceVM
                {
                    EvaluationCriteriaList = new List<PerDetailEvaluationMeasurementVM>()
                };

                // Basic Info
                if (persons.TryGetValue(first.EmployeeId, out var person))
                {
                    vm.EmployeeId = person.EmployeeCode;
                    vm.FullName = $"{person.EmployeeCode}-{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}";
                    vm.Sex = person.Gender.ToString();
                }
                if (quarterLookup.TryGetValue(first.GlPeriodId, out var quarter))
                {
                    vm.Period = quarter.Name;
                    vm.EvaluationFrom = quarter?.DateFrom?.ToString("MMMM dd, yyyy") ?? "";
                    vm.EvaluationTo = quarter?.DateTo?.ToString("MMMM dd, yyyy") ?? "";
                    vm.FullName = $"{person.EmployeeCode}-{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}";
                    vm.Sex = person.Gender.ToString();
                }

                if (departments.TryGetValue(first.OrgStructureId, out var dept))
                    vm.Department = dept;

                if (titles.TryGetValue(first.JobTitleId, out var title))
                    vm.JobTitle = title;
                int i = 1;
                // Loop employee performances
                foreach (var perf in employeeGroup)
                {
                    if (!measurementsByPerformance.TryGetValue(perf.EmployeePerformanceId, out var employeeMeasurements))
                        continue;


                    foreach (var measurement in employeeMeasurements)
                    {
                        if (!scoresByMeasurement.TryGetValue(measurement.EmpMeasurementId, out var measureScores))
                            continue;
                      
                        criteriaLookup.TryGetValue(perf.EvaluationCriteriaTypeId, out var criteria);
                        measurementLookup.TryGetValue(measurement.MeasurmentId, out var measurementName);

                        var detail = new PerDetailEvaluationMeasurementVM
                        {
                            EvaluationCriteriaList = new List<PerDetailEvaluationCriteriaVM>(),
                            Period = quarters.TryGetValue(perf.GlPeriodId, out var q) ? q : "",
                            Measurement = $"{i}.  {criteria?.EvaluationCriteria} {measurementName?.MeasurementName}",
                            TotalScore = measureScores.Sum(x => x.Score),
                            TotalWeight = measureScores.Sum(x => x.Weight)
                        };
                        var j = 1;
                        foreach (var score in measureScores)
                        {
                            actionLookup.TryGetValue(score.PerformanceActionId, out var action);

                            detail.EvaluationCriteriaList.Add(new PerDetailEvaluationCriteriaVM
                            {
                                Measurement = $"{i}.{j} {action?.ActionName}",
                                Weight = score.Weight,
                                Score = score.Score
                            });
                            j++;
                        }
                        detail.Score4 = detail.EvaluationCriteriaList
                                              .Where(x => x.Score == 4)
                                              .Sum(x => x.Score);

                        detail.Score8 = detail.EvaluationCriteriaList
                                            .Where(x => x.Score == 8)
                                            .Sum(x => x.Score);

                        detail.Score12 = detail.EvaluationCriteriaList
                                            .Where(x => x.Score == 12)
                                            .Sum(x => x.Score);

                        detail.Score16 = detail.EvaluationCriteriaList
                                            .Where(x => x.Score == 16)
                                            .Sum(x => x.Score);

                        detail.Score20 = detail.EvaluationCriteriaList
                                            .Where(x => x.Score == 20)
                                            .Sum(x => x.Score);

                        detail.TotalScore = detail.EvaluationCriteriaList
                                            .Sum(x => x.Score);
                        detail.TotalWeight = detail.EvaluationCriteriaList.Sum(x => x.Weight);
                        detail.AverageScore = detail.TotalScore/detail.EvaluationCriteriaList.Count();
                        vm.EvaluationCriteriaList.Add(detail);
                    }
                    i++;
                }

                // Calculate Totals
                var kpiCount = vm.EvaluationCriteriaList.Count;

                vm.TotalScore = vm.EvaluationCriteriaList.Sum(x => x.TotalScore);
                vm.TotalWeight = vm.EvaluationCriteriaList.Sum(x => x.TotalWeight);
                vm.AverageScore = vm.EvaluationCriteriaList.Average(x => x.AverageScore);
                vm.Percent = vm.TotalWeight>0?(vm.TotalScore / vm.TotalWeight)*100:0;
                vm.AverageScore = vm.TotalScore/ kpiCount;
                
                var rank = perfRanks.FirstOrDefault(x =>
                    x.Minimum <= vm.Percent && x.Maximum >= vm.Percent);

                if (rank != null)
                    vm.PerformanceRank = rank.Description;

                result.Add(vm);
            }

            if (!string.IsNullOrEmpty(rankName))
                result = result.Where(x => x.PerformanceRank == rankName).ToList();

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SharedDataSources.EmployeeViewModel> GetAllActivePlacedEmployees(string branchId,string orgStructureId,string year,
                                                                                      string quarterId, string status,string businessUnit)
        {
            int.TryParse(year, out int yearId);
            int.TryParse(quarterId, out int quarterIdInt);
            int.TryParse(branchId, out int branchIdInt);
            int.TryParse(orgStructureId, out int orgIdInt);
            int.TryParse(businessUnit, out int businessUnitId);

            // Get Fiscal Year Id (single value expected)
            int? fiscalYearId = db.QuarterLengths
                .Where(x => x.QuarterLengthId == yearId)
                .Select(x => (int?)x.GlFiscalYearId)
                .FirstOrDefault();

            var query =
                from performance in db.EmployeePerformances
                join employee in db.Person_Employee
                    on performance.EmployeeId equals employee.EmployeeId
                where performance.IsSecondLevelApproved
                      && employee.IsActive
                select new { performance, employee };

            // Apply filters dynamically
            if (fiscalYearId.HasValue)
                query = query.Where(x => x.performance.GlFiscalYearId == fiscalYearId.Value);

            if (quarterIdInt > 0)
                query = query.Where(x => x.performance.GlPeriodId == quarterIdInt);

            if (branchIdInt > 0)
                query = query.Where(x => x.performance.BranchId == branchIdInt);

            if (orgIdInt > 0)
                query = query.Where(x => x.performance.OrgStructureId == orgIdInt);

            if (businessUnitId > 0)
                query = query.Where(x => x.performance.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.performance.ResponsibilityLevel.ToString() == status);

            return query
                .Select(x => new SharedDataSources.EmployeeViewModel
                {
                    EmpId = x.employee.EmployeeId,

                    FullName = x.employee.EmployeeCode + "-" +
                               x.employee.FirstNameEnglish + " " +
                               x.employee.MiddleNameEnglish + " " +
                               x.employee.LastNameEnglish,

                    FullAmharicName = x.employee.EmployeeCode + "-" +
                                      x.employee.FirstName + " " +
                                      x.employee.MiddleName + " " +
                                      x.employee.LastName,

                    EmpCodeWithName = x.employee.EmployeeCode + "-" +
                                      x.employee.FirstName + " " +
                                      x.employee.MiddleName + " " +
                                      x.employee.LastName,

                    EmpCodeWithNameEng = x.employee.EmployeeCode + "_" +
                                         x.employee.FirstNameEnglish + " " +
                                         x.employee.MiddleNameEnglish + " " +
                                         x.employee.LastNameEnglish
                })
                .Distinct()
                .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MessageViewModel> GetMessages()
        {
            var performanceRanks = db.PerformanceRank
                                     .OrderByDescending(x => x.Maximum)
                                     .ToList();

            var messageParts = performanceRanks
                .Select(x =>
                    x.Minimum == 0
                        ? $"{x.Description} (<{x.Maximum + 1})"
                        : $"{x.Description} ({x.Minimum}-{x.Maximum})"
                );

            string finalMessage = "Performance Result Rating System: " +string.Join(", ", messageParts);

            return new List<MessageViewModel>
            {
                 new MessageViewModel { Message = finalMessage }
            };
        }

        public class MessageViewModel
        {
            public string Message { get; set; }
        }

        //public List<SharedDataSources.EmployeeViewModel> GetAllActivePlacedEmployees(string branchId, string orgStructureId, string year, string quarterId, string status, string BusinessUnit, )
        //{
        //    List<SharedDataSources.EmployeeViewModel> list = new List<SharedDataSources.EmployeeViewModel>();

        //    int.TryParse(year, out int yearId);
        //    int.TryParse(quarterId, out int quarterIdInt);
        //    int.TryParse(branchId, out int branchIdInt);
        //    int.TryParse(orgStructureId, out int orgIdInt);
        //    int.TryParse(BusinessUnit, out int businessUnitId);

        //    //Get Fiscal Year
        //    var fiscalYearIds = db.QuarterLengths
        //        .Where(x => x.QuarterLengthId == yearId)
        //        .Select(x => x.GlFiscalYearId)
        //        .ToList();

        //    //Base Query
        //    var query = db.EmployeePerformances
        //        .Where(x => x.IsSecondLevelApproved);

        //    if (fiscalYearIds.Any())
        //        query = query.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));

        //    if (quarterIdInt > 0)
        //        query = query.Where(x => x.GlPeriodId == quarterIdInt);

        //    if (branchIdInt > 0)
        //        query = query.Where(x => x.BranchId == branchIdInt);

        //    if (orgIdInt > 0)
        //        query = query.Where(x => x.OrgStructureId == orgIdInt);

        //    if (businessUnitId > 0)
        //        query = query.Where(x => x.BusinessUnitId == businessUnitId);

        //    if (!string.IsNullOrEmpty(status))
        //        query = query.Where(x => x.ResponsibilityLevel.ToString() == status);

        //    var performances = query.ToList();
        //    HashSet<int> perEmpIds = new HashSet<int>(performances.Select(x => x.EmployeeId));
        //    var employees = db.Person_Employee.Where(x => x.IsActive&&perEmpIds.Contains(x.EmployeeId));

        //    foreach (var employee in employees)
        //    {
        //        var employeeList = new SharedDataSources.EmployeeViewModel();
        //        employeeList.FullName = employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
        //        employeeList.FullAmharicName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
        //        employeeList.EmpId = employee.EmployeeId;
        //        employeeList.EmpCodeWithName = employee.EmployeeCode + "-" + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
        //        employeeList.EmpCodeWithNameEng = employee.EmployeeCode + "_" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
        //        list.Add(employeeList);
        //    }

        //    return list;
        //}

        //public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId,
        //                                                                    string year, string quarterId, string status, string BusinessUnit, string rankName)
        //{
        //    var employeePerformanceList = new List<EmployeePerfromanceVM>();
        //    decimal totalResult = 0;

        //    if (string.IsNullOrEmpty(year))
        //        return employeePerformanceList;

        //    if (string.IsNullOrEmpty(quarterId))
        //        return employeePerformanceList;

        //    int.TryParse(year, out int yearId);

        //    // Step 1: Filter early
        //    var fiscalYearIds = db.QuarterLengths
        //        .Where(x => x.QuarterLengthId == yearId)
        //        .Select(x => x.GlFiscalYearId)
        //        .ToList();

        //    var query = db.EmployeePerformances
        //        .Where(x => x.IsSecondLevelApproved);

        //    if (fiscalYearIds.Any())
        //        query = query.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));

        //    if (int.TryParse(quarterId, out int quartId))
        //        query = query.Where(x => x.GlPeriodId == quartId);

        //    if (int.TryParse(branchId, out int braId))
        //        query = query.Where(x => x.BranchId == braId);

        //    if (int.TryParse(orgStructureId, out int orgId))
        //        query = query.Where(x => x.OrgStructureId == orgId);

        //    if (int.TryParse(BusinessUnit, out int businessUnitId))
        //        query = query.Where(x => x.BusinessUnitId == businessUnitId);

        //    if (!string.IsNullOrEmpty(status))
        //        query = query.Where(x => x.ResponsibilityLevel.ToString() == status);

        //    // Step 2: Materialize only filtered data once
        //    var empPerformances = query.ToList();

        //    if (!empPerformances.Any())
        //        return employeePerformanceList;

        //    // Step 3: Preload all related data at once
        //    var employeeIds = empPerformances.Select(x => x.EmployeeId).Distinct().ToList();
        //    var orgIds = empPerformances.Select(x => x.OrgStructureId).Distinct().ToList();
        //    var titleIds = empPerformances.Select(x => x.JobTitleId).Distinct().ToList();
        //    var glPeriodIds = empPerformances.Select(x => x.GlPeriodId).Distinct().ToList();
        //    var perfIds = empPerformances.Select(x => x.EmployeePerformanceId).ToList();

        //    var persons = db.Person_Employee
        //        .Where(x => employeeIds.Contains(x.EmployeeId))
        //        .ToDictionary(x => x.EmployeeId, x => x);

        //    var departments = db.MainCompanyStructures
        //        .Where(x => orgIds.Contains(x.OrgStructureId))
        //        .ToDictionary(x => x.OrgStructureId, x => x.Name);

        //    var titles = db.Lookups
        //        .Where(x => titleIds.Contains(x.LookupId))
        //        .ToDictionary(x => x.LookupId, x => x.Description);

        //    var quarters = db.Quarters
        //        .Where(x => glPeriodIds.Contains(x.QuarterId))
        //        .ToDictionary(x => x.QuarterId, x => x.Name);

        //    var perfRanks = db.PerformanceRank.ToList();

        //    // Preload measurements and scores for all performances
        //    var allMeasurements = db.EmployeeMeasurement
        //        .Where(x => perfIds.Contains(x.EmployeePerformanceId))
        //        .ToList();

        //    var measureIds = allMeasurements.Select(x => x.EmpMeasurementId).ToList();
        //    var allScores = db.EmployeePerformanceScores
        //        .Where(x => measureIds.Contains(x.EmployeeMeasurementId))
        //        .ToList();

        //    var allCriteria = db.MeasurementDefinitions
        //        .ToDictionary(x => x.MeasurementDefinitionId, x => x);
        //    var measurements = db.Measurements
        //        .ToDictionary(x => x.MeasurementId, x => x);
        //    var measurementDetails = db.PerformanceActions
        //        .ToDictionary(x => x.ActionId, x => x);

        //    // Step 4: Group data by Employee
        //    var grouped = empPerformances.GroupBy(x => x.EmployeeId);

        //    foreach (var group in grouped)
        //    {

        //        var place = group.First();
        //        var employeePerformance = new EmployeePerfromanceVM
        //        {
        //            EvaluationCriteriaList = new List<PerDetailEvaluationMeasurementVM>()
        //        };

        //        // Person info
        //        if (persons.TryGetValue(place.EmployeeId, out var person))
        //        {
        //            employeePerformance.EmployeeId = person.EmployeeCode;
        //            employeePerformance.Sex = person.Gender.ToString();
        //            employeePerformance.FullName = person.EmployeeCode+"-"+person.FirstNameEnglish +" "+person.MiddleNameEnglish+" "+person.LastNameEnglish;
        //        }

        //        if (departments.TryGetValue(place.OrgStructureId, out var deptName))
        //            employeePerformance.Department = deptName;

        //        if (titles.TryGetValue(place.JobTitleId, out var title))
        //            employeePerformance.JobTitle = title;

        //        // Step 5: Loop by quarter
        //        var employeePerformancePerEmployee = empPerformances.Where(x => x.EmployeeId == place.EmployeeId).ToList();
        //        foreach (var quarterGroup in employeePerformancePerEmployee)
        //        {



        //             var allMeasurementPerCriterias = allMeasurements.Where(m => m.EmployeePerformanceId == quarterGroup.EmployeePerformanceId);
        //            // Each performance in that period
        //            foreach (var measurementPerCriteria in allMeasurementPerCriterias)
        //            {

        //                allCriteria.TryGetValue(quarterGroup.EvaluationCriteriaTypeId, out var qCriteria);
        //                measurements.TryGetValue(measurementPerCriteria.MeasurmentId, out var measurement);
        //                var evaluationPeriod = new PerDetailEvaluationMeasurementVM
        //                {
        //                    EvaluationCriteriaList = new List<PerDetailEvaluationCriteriaVM>(),
        //                    Period = quarters.TryGetValue(quarterGroup.GlPeriodId, out var qName) ? qName : "",
        //                    Measurement = qCriteria.EvaluationCriteria+ " "+ measurement.MeasurementName,
        //                    TotalScore = allScores.Where(s => s.EmployeeMeasurementId == measurementPerCriteria.EmpMeasurementId).Sum(x=>x.Score),
        //                    TotalWeight = allScores.Where(s => s.EmployeeMeasurementId == measurementPerCriteria.EmpMeasurementId).Sum(x=>x.Weight),
        //                };
        //                if (!allMeasurements.Any(m => m.EmployeePerformanceId == measurementPerCriteria.EmployeePerformanceId))
        //                    continue;

        //                var MeasurePerScores = allScores
        //                    .Where(m => m.EmployeeMeasurementId == measurementPerCriteria.EmpMeasurementId).ToList();


        //                foreach (var measure in MeasurePerScores)
        //                {

        //                    measurementDetails.TryGetValue(measure.PerformanceActionId, out var performanceAction);

        //                    var vm = new PerDetailEvaluationCriteriaVM
        //                    {
        //                        Measurement = $"{performanceAction.ActionName}",
        //                        Weight = measure.Weight,
        //                        Score = measure.Score
        //                    };
        //                    evaluationPeriod.EvaluationCriteriaList.Add(vm);
        //                }
        //            }


        //        }

        //        if (employeePerformance.EvaluationCriteriaList.Any())
        //        {
        //            employeePerformance.TotalScoreInThePeriod = employeePerformance.EvaluationCriteriaList.Sum(x => x.TotalScore);
        //            employeePerformance.TotalWeight = employeePerformance.EvaluationCriteriaList.Sum(x => x.TotalWeight);
        //            employeePerformance.Percent = employeePerformance.TotalWeight > 0
        //                ? (employeePerformance.TotalScore * 100) / employeePerformance.TotalWeight
        //                : 0;

        //            var rank = perfRanks.FirstOrDefault(x =>
        //                x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);

        //            if (rank != null)
        //                employeePerformance.PerformanceRank = rank.Description;

        //            totalResult += employeePerformance.TotalScore;
        //        }

        //        employeePerformanceList.Add(employeePerformance);
        //    }

        //    if (!string.IsNullOrEmpty(rankName))
        //        employeePerformanceList = employeePerformanceList
        //            .Where(x => x.PerformanceRank == rankName)
        //            .ToList();

        //    return employeePerformanceList;
        //}




        public void Dispose()
        {
            db.Dispose();
        }
    }
}