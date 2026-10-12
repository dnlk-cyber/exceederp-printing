using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTypeSummaryObjectDataSource:IDisposable
    {
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTypeSummaryVM> GetEmployeeTypeSummary(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        {
            // Get fiscal year info once
            var fyearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = db.GLFiscalYears.Find(fyearId);

            DateTime today = DateTime.Today;
            DateTime currentYearStart = fiscalYear?.DateFrom ?? new DateTime(today.Year, 1, 1);
            DateTime currentYearEnd = fiscalYear?.DateTo ?? new DateTime(today.Year, 12, 31);
            DateTime previousYearStart = currentYearStart.AddYears(-1);
            DateTime previousYearEnd = currentYearEnd.AddYears(-1);

            // Build the base query for current employees
            var empPlacementsQuery = db.Placements.AsQueryable();

            // Apply filters
            if (status == "Active")
            {
                empPlacementsQuery = empPlacementsQuery.Where(x => x.isCurrent);
            }
            else if (status == "InActive")
            {
                empPlacementsQuery = empPlacementsQuery.Where(x => !x.isCurrent);
            }

            if (int.TryParse(branchId, out int braId))
                empPlacementsQuery = empPlacementsQuery.Where(x => x.BranchId == braId);

            if (int.TryParse(businessUnitId, out int bUnitId))
                empPlacementsQuery = empPlacementsQuery.Where(x => x.BusinessUnitId == bUnitId);

            if (int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures
                    .Where(x => x.Parent == orgId)
                    .Select(x => x.OrgStructureId)
                    .ToList();
                empPlacementsQuery = empPlacementsQuery.Where(x => structureIds.Contains(x.OrgStructureId));
            }

            if (int.TryParse(job, out int lookUpId))
                empPlacementsQuery = empPlacementsQuery.Where(x => x.LookupId == lookUpId);

            // Get distinct employee IDs from placements
            var currentEmployeeIds = empPlacementsQuery
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // Get terminated employees from previous year
            var terminatedEmployeeIds = db.Terminations
                .Where(x => x.IsOnlineApproved &&
                            x.TerminationDate >= previousYearStart &&
                            x.TerminationDate <= previousYearEnd)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // Combine current and terminated employee IDs for previous year query
            var allPreviousEmployeeIds = currentEmployeeIds.Union(terminatedEmployeeIds).ToList();

            // Get current employees with one query
            var currentEmployees = db.Person_Employee
                .Where(e => currentEmployeeIds.Contains(e.EmployeeId))
                .ToList();

            // Get previous year employees with one query
            var previousEmployees = db.Person_Employee
                .Where(e => allPreviousEmployeeIds.Contains(e.EmployeeId) &&
                            e.HiredDate <= currentYearStart)
                .ToList();

            // Get employee types once
            var employeeTypes = db.EmployeeTypes.ToList();

            // Group current employees by status
            var groupedEmployees = currentEmployees.Where(x=>x.Status != EmployeeStatus.NOTSELECTED)
                .GroupBy(e => e.Status)
                .ToList();

            var result = new List<EmployeeTypeSummaryVM>();
            foreach (var group in groupedEmployees)
            {
                var employees = group.ToList();
                var employeeIds = employees.Select(e => e.EmployeeId).ToList();

                var summary = new EmployeeTypeSummaryVM
                {
                    EmployeeType = group.Key.ToString(),
                    Female = "Female",
                    TotalFemale = employees.Count(e => e.Gender == Gender.FEMALE),
                    PreviousTotalFemale = previousEmployees.Count(e => e.Gender == Gender.FEMALE && employeeIds.Contains(e.EmployeeId)),
                    Male = "Male",
                    TotalMale = employees.Count(e => e.Gender == Gender.MALE),
                    PreviousTotalMale = previousEmployees.Count(e => e.Gender == Gender.MALE && employeeIds.Contains(e.EmployeeId)),
                    TotalEmployee = employees.Count,
                    PreviousTotalEmployee = previousEmployees.Count(e => employeeIds.Contains(e.EmployeeId))
                };

                summary.EmployeeTypeDetails = employeeTypes.Select(type =>
                {
                    var typeEmployees = employees.Where(e => e.EmployeeTypeId == type.EmployeeTypeId).ToList();
                    var typeEmployeeIds = typeEmployees.Select(e => e.EmployeeId).ToList();

                    return new EmployeeTypeDetailSummaryVM
                    {
                        EmployeeType = type.Name,
                        Female = "Female",
                        TotalFemale = typeEmployees.Count(e => e.Gender == Gender.FEMALE),
                        PreviousTotalFemale = previousEmployees.Count(e => e.Gender == Gender.FEMALE && e.EmployeeTypeId == type.EmployeeTypeId && typeEmployeeIds.Contains(e.EmployeeId)),
                        Male = "Male",
                        TotalMale = typeEmployees.Count(e => e.Gender == Gender.MALE),
                        PreviousTotalMale = previousEmployees.Count(e => e.Gender == Gender.MALE && e.EmployeeTypeId == type.EmployeeTypeId && typeEmployeeIds.Contains(e.EmployeeId)),
                        TotalEmployee = typeEmployees.Count,
                        PreviousTotalEmployee = previousEmployees.Count(e => e.EmployeeTypeId == type.EmployeeTypeId && typeEmployeeIds.Contains(e.EmployeeId))
                    };
                }).ToList();

                result.Add(summary);
            }

            return result;
        }
        //public List<EmployeeTypeSummaryVM> GetEmployeeTypeSummary(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        //{
        //    var fyearId = _fiscalYearHelper.GetCurrentFYearId();
        //    var fiscalYear = db.GLFiscalYears.Find(fyearId);

        //    DateTime today = DateTime.Today;
        //    DateTime defaultStart = new DateTime(today.Year, 1, 1);
        //    DateTime defaultEnd = new DateTime(today.Year, 12, 31);

        //    DateTime currentYearStart = fiscalYear?.DateFrom ?? defaultStart;
        //    DateTime currentYearEnd = fiscalYear?.DateTo ?? defaultEnd;
        //    DateTime previousYearStart = currentYearStart.AddYears(-1);
        //    DateTime previousYearEnd = currentYearEnd.AddYears(-1);

        //    var empPlacementsQuery = db.Placements.AsQueryable();
        //    var employeesQuery = db.Person_Employee.AsQueryable();

        //    if (status == "Active")
        //    {
        //        employeesQuery = employeesQuery.Where(x => x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.isCurrent);
        //    }
        //    else if (status == "InActive")
        //    {
        //        employeesQuery = employeesQuery.Where(x => !x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => !x.isCurrent);
        //    }

        //    if (int.TryParse(branchId, out int braId))
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BranchId == braId);

        //    if (int.TryParse(businessUnitId, out int bUnitId))
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BusinessUnitId == bUnitId);

        //    if (int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures
        //            .Where(x => x.Parent == orgId)
        //            .Select(x => x.OrgStructureId).ToList();

        //        empPlacementsQuery = empPlacementsQuery.Where(x => structureIds.Contains(x.OrgStructureId));
        //    }

        //    if (int.TryParse(job, out int lookUpId))
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.LookupId == lookUpId);

        //    var employeeIds = empPlacementsQuery.Select(x => x.EmployeeId).Distinct().ToList().Distinct();
        //    employeesQuery = employeesQuery.Where(x => employeeIds.Contains(x.EmployeeId));

        //    var filteredEmployees = employeesQuery.ToList();





        //    //add currently terminated Employee but they are employee of the previous year
        //    var terminatedEmployeeIds = db.Terminations
        //                                        .Where(x => x.IsOnlineApproved
        //                                                 && x.TerminationDate >= previousYearStart
        //                                                 && x.TerminationDate <= previousYearEnd)
        //                                                 .Select(x=>x.EmployeeId).Distinct();

        //    // Fetch terminated or all previous-year employees regardless of current status
        //    var totalPreviousEmployees = db.Person_Employee
        //        .Where(e => e.HiredDate <= currentYearStart 
        //                 && (employeeIds.Contains(e.EmployeeId)
        //                 || terminatedEmployeeIds.Contains(e.EmployeeId)))
        //                 .DistinctByField(x=>x.EmployeeId).ToList();
        //    var employeeTypes = db.EmployeeTypes.ToList();
        //    var groupedEmployees = filteredEmployees
        //        .GroupBy(e => e.Status)
        //        .ToList();

        //    var list = new List<EmployeeTypeSummaryVM>();

        //    foreach (var group in groupedEmployees)
        //    {
        //        var employees = group.ToList();

        //        var summary = new EmployeeTypeSummaryVM
        //        {
        //            EmployeeType = group.Key.ToString(),
        //            Female = "Female",
        //            TotalFemale = employees.Count(e => e.Gender == Gender.FEMALE /*&& e.IsActive*/),
        //            PreviousTotalFemale = totalPreviousEmployees.Count(e => e.Gender == Gender.FEMALE /*&& employees.Any(c => c.EmployeeId == e.EmployeeId)*/),
        //            Male = "Male",
        //            TotalMale = employees.Count(e => e.Gender == Gender.MALE /*&& e.IsActive*/),
        //            PreviousTotalMale = totalPreviousEmployees.Count(e => e.Gender == Gender.MALE /*&& employees.Any(c => c.EmployeeId == e.EmployeeId)*/),
        //            TotalEmployee = employees.Count(/*e=> e.IsActive*/),
        //            PreviousTotalEmployee = totalPreviousEmployees.Count(e => employees.Any(c => c.EmployeeId == e.EmployeeId))
        //        };

        //        summary.EmployeeTypeDetails = employeeTypes.Select(type => new EmployeeTypeDetailSummaryVM
        //        {
        //            EmployeeType = type.Name,
        //            Female = "Female",
        //            TotalFemale = employees.Count(e => e.Gender == Gender.FEMALE && e.EmployeeTypeId == type.EmployeeTypeId/*&&e.IsActive*/),
        //            PreviousTotalFemale = totalPreviousEmployees.Count(e => e.Gender == Gender.FEMALE && e.EmployeeTypeId == type.EmployeeTypeId /*&& employees.Any(c => c.EmployeeId == e.EmployeeId)*/),
        //            Male = "Male",
        //            TotalMale = employees.Count(e => e.Gender == Gender.MALE && e.EmployeeTypeId == type.EmployeeTypeId /*&& e.IsActive*/),
        //            PreviousTotalMale = totalPreviousEmployees.Count(e => e.Gender == Gender.MALE && e.EmployeeTypeId == type.EmployeeTypeId /*&& employees.Any(c => c.EmployeeId == e.EmployeeId)*/),
        //            TotalEmployee = employees.Count(e => e.EmployeeTypeId == type.EmployeeTypeId /*&& e.IsActive*/),
        //            PreviousTotalEmployee = totalPreviousEmployees.Count(e => e.EmployeeTypeId == type.EmployeeTypeId /*&& employees.Any(c => c.EmployeeId == e.EmployeeId)*/)
        //        }).ToList();

        //        list.Add(summary);
        //    }

        //    return list;
        //}

        //public List<EmployeeTypeSummaryVM> GetEmployeeTypeSummary(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        //{
        //    var fyearId = _fiscalYearHelper.GetCurrentFYearId();
        //    var fiscalYear = db.GLFiscalYears.Find(fyearId);

        //    DateTime today = DateTime.Today;
        //    DateTime defaultStart = new DateTime(today.Year, 1, 1);
        //    DateTime defaultEnd = new DateTime(today.Year, 12, 31);

        //    // If fiscal year is found, use its start/end, else fallback to calendar year
        //    DateTime currentYearStart = fiscalYear?.DateFrom ?? defaultStart;
        //    DateTime currentYearEnd = fiscalYear?.DateTo ?? defaultEnd;

        //    // Previous fiscal year: just subtract 1 year
        //    DateTime previousYearStart = currentYearStart.AddYears(-1);
        //    DateTime previousYearEnd = currentYearEnd.AddYears(-1);

        //    var list = new List<EmployeeTypeSummaryVM>();
        //    var empPlacementsQuery = db.Placements.AsQueryable();
        //    var employeesQuery = db.Person_Employee.AsQueryable();

        //    // Filter employees and placements by status
        //    if (status == "Active")
        //    {
        //        employeesQuery = employeesQuery.Where(x => x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.isCurrent);
        //    }
        //    else if (status == "InActive")
        //    {
        //        employeesQuery = employeesQuery.Where(x => !x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => !x.isCurrent);
        //    }

        //    // Filter placements by branch, business unit, and organizational structure
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BranchId == braId);
        //    }
        //    if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BusinessUnitId == bUnitId);
        //    }
        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
        //        empPlacementsQuery = empPlacementsQuery.Where(x => structureIds.Contains(x.OrgStructureId));
        //    }
        //    if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.LookupId == lookUpId);
        //    }

        //    // Join employees with placements
        //    var employeeIds = empPlacementsQuery.Select(x => x.EmployeeId).Distinct().ToList();
        //    employeesQuery = employeesQuery.Where(x => employeeIds.Contains(x.EmployeeId));

        //    // Group employees by status and employee type
        //    var groupedEmployees = employeesQuery
        //        .GroupBy(e => e.Status)
        //        .Select(g => new
        //        {
        //            Status = g.Key,
        //            Employees = g.ToList()
        //        })
        //        .ToList();

        //    var employeeTypes = db.EmployeeTypes.ToList(); // Fetch all employee types once

        //    foreach (var group in groupedEmployees)
        //    {
        //        var statusName = group.Status.ToString();
        //        var employees = group.Employees;
        //        var totalPerviousEmployees = employees
        //            .Where(e => e.HiredDate <= currentYearStart)
        //           .ToList();
        //        var summary = new EmployeeTypeSummaryVM
        //        {
        //            EmployeeType = statusName,
        //            Female = "Female",
        //            TotalFemale = employees.Count(e => e.Gender == Gender.FEMALE),
        //            PreviousTotalFemale = totalPerviousEmployees.Count(e => e.Gender == Gender.FEMALE),
        //            Male = "Male",
        //            TotalMale = employees.Count(e => e.Gender == Gender.MALE),
        //            PreviousTotalMale = totalPerviousEmployees.Count(e => e.Gender == Gender.MALE),
        //            PreviousTotalEmployee = totalPerviousEmployees.Count(),
        //            TotalEmployee = employees.Count()
        //        };

        //        summary.EmployeeTypeDetails = employeeTypes.Select(employeeType => new EmployeeTypeDetailSummaryVM
        //        {
        //            EmployeeType = employeeType.Name,
        //            Female = "Female",
        //            TotalFemale = employees.Count(e => e.Gender == Gender.FEMALE && e.EmployeeTypeId == employeeType.EmployeeTypeId),
        //            PreviousTotalFemale = totalPerviousEmployees.Count(e => e.Gender == Gender.FEMALE && e.EmployeeTypeId == employeeType.EmployeeTypeId),
        //            Male = "Male",
        //            TotalMale = employees.Count(e => e.Gender == Gender.MALE && e.EmployeeTypeId == employeeType.EmployeeTypeId
        //                                            && e.EmployeeTypeId == employeeType.EmployeeTypeId),
        //            TotalEmployee = employees.Count(e => e.EmployeeTypeId == employeeType.EmployeeTypeId),
        //            PreviousTotalMale = totalPerviousEmployees.Count(e => e.Gender == Gender.MALE
        //                                                               && e.EmployeeTypeId == employeeType.EmployeeTypeId),
        //            PreviousTotalEmployee = totalPerviousEmployees.Count(e => e.EmployeeTypeId == employeeType.EmployeeTypeId)
        //        }).ToList();

        //        list.Add(summary);
        //    }

        //    return list;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}