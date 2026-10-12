using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeResponsiblityLevelSummaryObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StaffCountViewModel> GetEmployees(string branchId, object[] orgStructureIds, string businessUnit, string status)
        {
            // -------------------------------
            // Current placements only
            // -------------------------------
            IQueryable<Placement> placementQuery = _db.Placements;

            // -------------------------------
            // Employees Query
            // -------------------------------
            IQueryable<Person_Employee> employeeQuery = _db.Person_Employee;

            if (status == "Active")
            {
                employeeQuery = employeeQuery.Where(e => e.IsActive);
                placementQuery = placementQuery.Where(e => e.isCurrent);
            }
            else if (status == "InActive")
            {
                employeeQuery = employeeQuery.Where(e => !e.IsActive);
                placementQuery = placementQuery.Where(e => !e.isCurrent);
            }

            // -------------------------------
            // Branch Filter
            // -------------------------------
            if (int.TryParse(branchId, out int branch))
            {
                placementQuery = placementQuery
                    .Where(p => p.BranchId == branch);
            }

            // -------------------------------
            // Org Structure Filter
            // -------------------------------
            if (orgStructureIds != null && orgStructureIds.Length > 0)
            {
                var orgIds = orgStructureIds
                    .Select(x => int.TryParse(x?.ToString(), out int id) ? id : (int?)null)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .Distinct()
                    .ToList();

                if (orgIds.Any())
                {
                    placementQuery = placementQuery
                        .Where(p => orgIds.Contains(p.OrgStructureId));
                }
            }

            // -------------------------------
            // Business Unit Filter
            // -------------------------------
            if (int.TryParse(businessUnit, out int buId))
            {
                placementQuery = placementQuery
                    .Where(p => p.BusinessUnitId == buId);
            }

         
            // -------------------------------
            // Join current placement + employee
            // -------------------------------
            var data = (
                from p in placementQuery

                join e in employeeQuery
                    on p.EmployeeId equals e.EmployeeId

                select new
                {
                    p.EmployeeId,
                    p.ResponsibilityLevel,

                    e.Gender,
                    e.Status
                }
            )
            .Distinct()
            .ToList();

            // -------------------------------
            // Probation Employees
            // -------------------------------
            var employeeIds = data
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            var employeeList = _db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToList();

            var probationIds = HrHelper.GetEmployeeInProbaition(employeeList);

            // -------------------------------
            // Grouping
            // -------------------------------
            var result = data
                .GroupBy(x => x.ResponsibilityLevel)
                .Select(g => new StaffCountViewModel
                {
                    ResponsibilityLevel = g.Key != null
                        ? g.Key.ToString()
                        : "Unknown",

                    PermanentMale = g.Count(x =>
                        !probationIds.Contains(x.EmployeeId) &&
                        x.Gender == Gender.MALE &&
                        x.Status == EmployeeStatus.PERMANENT),

                    PermanentFemale = g.Count(x =>
                        !probationIds.Contains(x.EmployeeId) &&
                        x.Gender == Gender.FEMALE &&
                        x.Status == EmployeeStatus.PERMANENT),

                    TemporaryMale = g.Count(x =>
                        !probationIds.Contains(x.EmployeeId) &&
                        x.Gender == Gender.MALE &&
                        x.Status == EmployeeStatus.CONTRACT),

                    TemporaryFemale = g.Count(x =>
                        !probationIds.Contains(x.EmployeeId) &&
                        x.Gender == Gender.FEMALE &&
                        x.Status == EmployeeStatus.CONTRACT)
                })
                .OrderBy(x => x.ResponsibilityLevel)
                .ToList();

            return result;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}

    //public class EmployeeResponsiblityLevelSummaryObjectDataSource : IDisposable
    //{
    //    private ExceedDbContext _db = new ExceedDbContext();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<StaffCountViewModel> GetEmployees(string branchId, object[] orgStructureIds, string BusinessUnit, string status)
    //    {
    //        var employees = GetFilteredEmployees(branchId, null);

    //        // Filter by org structure if needed
    //        if (orgStructureIds != null && orgStructureIds.Length > 0)
    //        {
    //            var orgStructIds = orgStructureIds
    //                .Select(id => int.TryParse(id?.ToString(), out var parsed) ? parsed : (int?)null)
    //                .Where(id => id.HasValue)
    //                .Select(id => id.Value)
    //                .ToList();

    //            var placementEmpIds = _db.Placements
    //                .Where(p => orgStructIds.Contains(p.OrgStructureId))
    //                .Select(p => p.EmployeeId)
    //                .Distinct()
    //                .ToList();

    //            employees = employees.Where(e => placementEmpIds.Contains(e.EmployeeId));
    //        }

    //        if(!string.IsNullOrWhiteSpace(BusinessUnit)&& int.TryParse(BusinessUnit, out int BusinessUnitId))
    //        {
    //            var placementEmpIds = _db.Placements
    //                .Where(p =>p.BusinessUnitId== BusinessUnitId )
    //                .Select(p => p.EmployeeId)
    //                .Distinct()
    //                .ToList();

    //            employees = employees.Where(e => placementEmpIds.Contains(e.EmployeeId));
    //        }
    //        if (status == "Active")
    //            employees = employees.Where(e => e.IsActive);
    //        else if (status == "InActive")
    //            employees = employees.Where(e => !e.IsActive);

    //        var employeeList = employees.ToList();
    //        var probationIds = HrHelper.GetEmployeeInProbaition(employeeList);

    //        var placements = _db.Placements.Where(p => p.isCurrent).ToList();

    //        var grouped = placements
    //            .Join(employeeList, p => p.EmployeeId, e => e.EmployeeId, (p, e) => new { p, e })
    //            .GroupBy(x => x.p.ResponsibilityLevel)
    //            .Select(g => new StaffCountViewModel
    //            {
    //                ResponsibilityLevel = g.Key.ToString(),
    //                PermanentMale = g.Count(x => !probationIds.Contains(x.e.EmployeeId)
    //                                             && x.e.Gender == Gender.MALE
    //                                             && x.e.Status == EmployeeStatus.PERMANENT),
    //                PermanentFemale = g.Count(x => !probationIds.Contains(x.e.EmployeeId)
    //                                               && x.e.Gender == Gender.FEMALE
    //                                               && x.e.Status == EmployeeStatus.PERMANENT),
    //                TemporaryMale = g.Count(x => !probationIds.Contains(x.e.EmployeeId)
    //                                             && x.e.Gender == Gender.MALE
    //                                             && x.e.Status == EmployeeStatus.CONTRACT),
    //                TemporaryFemale = g.Count(x => !probationIds.Contains(x.e.EmployeeId)
    //                                               && x.e.Gender == Gender.FEMALE
    //                                               && x.e.Status == EmployeeStatus.CONTRACT)
    //            })
    //            .ToList();

    //        return grouped;
    //    }
    //    private IQueryable<Person_Employee> GetFilteredEmployees(string branch, string businessUnitId)
    //    {
    //        var query = _db.Person_Employee.Where(x => x.IsActive);

    //        IQueryable<Placement> placements = _db.Placements.Where(p => p.isCurrent);

    //        if (!string.IsNullOrEmpty(branch) && int.TryParse(branch, out int branchId))
    //            placements = placements.Where(p => p.BranchId == branchId);

    //        if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int unitId))
    //            placements = placements.Where(p => p.BusinessUnitId == unitId);

    //        var empIds = placements.Select(p => p.EmployeeId);
    //        query = query.Where(e => empIds.Contains(e.EmployeeId));

    //        return query;
    //    }

    //public List<StaffCountViewModel> GetEmployees(string branchId, object[] orgStructureIds, string status)
    //{
    //    // Base query
    //    IQueryable<Placement> placements = db.Placements;
    //    IQueryable<Person_Employee> employees = db.Person_Employee;

    //    // Branch filter
    //    if (int.TryParse(branchId, out int braId))
    //    {
    //        placements = placements.Where(p => p.BranchId == braId);
    //    }

    //    // OrgStructure filter
    //    if (orgStructureIds != null && orgStructureIds.Length > 0)
    //    {
    //        var orgStructIds = orgStructureIds
    //            .Select(id => int.TryParse(id?.ToString(), out var parsed) ? parsed : (int?)null)
    //            .Where(id => id.HasValue)
    //            .Select(id => id.Value)
    //            .ToList();

    //        if (orgStructIds.Any())
    //            placements = placements.Where(p => orgStructIds.Contains(p.OrgStructureId));
    //    }

    //    // Status filter (active/inactive)
    //    bool? isActive = null;
    //    if (status == "Active") isActive = true;
    //    else if (status == "InActive") isActive = false;

    //    if (isActive.HasValue)
    //        placements = placements.Where(p => p.isCurrent == isActive.Value);

    //    // Join employees with placements (only filtered once)
    //    var query = from p in placements
    //                join e in employees on p.EmployeeId equals e.EmployeeId
    //                where !isActive.HasValue || e.IsActive == isActive.Value
    //                select new
    //                {
    //                    e.EmployeeId,
    //                    e.Gender,
    //                    e.IsActive,
    //                    p.EmployementType,
    //                    p.ResponsibilityLevel
    //                };

    //    // Execute single database call and group in memory
    //    var grouped = query
    //        .AsEnumerable() // Run DB query first
    //        .GroupBy(x => x.ResponsibilityLevel)
    //        .Select(g =>
    //        {
    //            return new StaffCountViewModel
    //            {
    //                ResponsibilityLevel = g.Key.ToString(),
    //                PermanentMale = g.Count(x => x.Gender == Gender.MALE && x.EmployementType == EmployeeStatus.PERMANENT),
    //                PermanentFemale = g.Count(x => x.Gender == Gender.FEMALE && x.EmployementType == EmployeeStatus.PERMANENT),
    //                TemporaryMale = g.Count(x => x.Gender == Gender.MALE && x.EmployementType == EmployeeStatus.CONTRACT),
    //                TemporaryFemale = g.Count(x => x.Gender == Gender.FEMALE && x.EmployementType == EmployeeStatus.CONTRACT)
    //            };
    //        })
    //        .ToList();

    //    // Ensure all ResponsibilityLevels appear (even with 0 count)
    //    var allTypes = Enum.GetValues(typeof(ResponsibilityLevel))
    //        .Cast<ResponsibilityLevel>()
    //        .Where(r => r != ResponsibilityLevel.NOTSELECTED)
    //        .Select(r => r.ToString())
    //        .ToList();

    //    foreach (var type in allTypes)
    //    {
    //        if (!grouped.Any(x => x.ResponsibilityLevel == type))
    //        {
    //            grouped.Add(new StaffCountViewModel
    //            {
    //                ResponsibilityLevel = type,
    //                PermanentMale = 0,
    //                PermanentFemale = 0,
    //                TemporaryMale = 0,
    //                TemporaryFemale = 0
    //            });
    //        }
    //    }

    //    return grouped.OrderBy(x => x.ResponsibilityLevel).ToList();
    //}


   

