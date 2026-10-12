using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerforNotEvaluatedEmployeeObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId, string year, string period, string status, string empId, string businessUnit)
        {
            db.Database.CommandTimeout = 7200;//added
            var employeePerformanceList = new List<EmployeePerfromanceVM>();

            if (string.IsNullOrEmpty(year))
            {
                return employeePerformanceList;
            }

            int.TryParse(year, out int yearId);
            var employeePerformances = db.EmployeePerformances.AsQueryable();

            var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId).ToList();
            if (GlFiscalYearIds.Any() && employeePerformances.Any())
            {
                employeePerformances = employeePerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId));
            }

            // Start with placements that are current
            var placements = db.Placements.Where(x => x.isCurrent).Select(x=>new {x.EmployeeId,x.BranchId,x.OrgStructureId,x.BusinessUnitId,x.AssignmentDate,x.AssignmentDateAmharic }).AsQueryable();

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId)&& placements.Any())
            {
                placements = placements.Where(x => x.BranchId == braId);
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId) && placements.Any())
            {
                placements = placements.Where(x => x.OrgStructureId == orgId);
            }

            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId) && placements.Any())
            {
                placements = placements.Where(x => x.EmployeeId == employeeId);
            }

            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId) && placements.Any())
            {
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId);
            }

            int periodId = 0;
            bool hasPeriod = !string.IsNullOrEmpty(period) && int.TryParse(period, out periodId);
            if (hasPeriod && employeePerformances.Any())
            {
                employeePerformances = employeePerformances.Where(x => x.GlPeriodId == periodId);
            }

            // Filter out employees already not evaluated
            if (placements.Any() && employeePerformances.Any())
            {
                if (!string.IsNullOrEmpty(status) && status.Trim().ToLower() == "reviewed")
                {
                    employeePerformances = employeePerformances.Where(x => !x.IsSecondLevelApproved);
                    var employeeIds = employeePerformances.Select(x => x.EmployeeId);
                    placements = placements.Where(x => employeeIds.Contains(x.EmployeeId));
                }
                if (!string.IsNullOrEmpty(status) && status.Trim().ToLower() == "notreviewed")
                {
                    var employeeIds = employeePerformances.Where(x => !x.IsSecondLevelApproved).Select(x => x.EmployeeId);
                    placements = placements.Where(x => !employeeIds.Contains(x.EmployeeId));
                }
                else
                {
                    var employeeIds = employeePerformances.Where(x => x.IsSecondLevelApproved).Select(x => x.EmployeeId);
                    placements = placements.Where(x => !employeeIds.Contains(x.EmployeeId));
                }
            }

            var placementList = placements.ToList();
            if (!placementList.Any())
            {
                return employeePerformanceList;
            }

            // Employees that have an evaluation still awaiting second-level approval
            var pendingQuery = db.EmployeePerformances.Where(x => !x.IsSecondLevelApproved && GlFiscalYearIds.Contains(x.GlFiscalYearId));
            if (hasPeriod)
            {
                pendingQuery = pendingQuery.Where(x => x.GlPeriodId == periodId);
            }
            var pendingEmployeeIds = new HashSet<int>(pendingQuery.Select(x => x.EmployeeId).Distinct().ToList());

            var employeeIdList = placementList.Select(x => x.EmployeeId).Distinct().ToList();
            var orgIds = placementList.Select(x => x.OrgStructureId).Distinct().ToList();
            var unitIds = placementList.Select(x => x.BusinessUnitId).Distinct().ToList();
            var branchIds = placementList.Select(x => x.BranchId).Distinct().ToList();

            var persons = db.Person_Employee.Where(x => employeeIdList.Contains(x.EmployeeId)).ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToDictionary(x => x.OrgStructureId, x => x.Name);
            var businessUnits = db.BusinessUnits.Where(x => unitIds.Contains(x.BusinessUnitId)).ToDictionary(x => x.BusinessUnitId, x => x.Name);
            var branches = db.Branch.Where(x => branchIds.Contains(x.BranchId)).ToDictionary(x => x.BranchId, x => x.Name);

            foreach (var place in placementList)
            {
                if (!persons.TryGetValue(place.EmployeeId, out var emp))
                {
                    continue;
                }

                var empPerformance = new EmployeePerfromanceVM
                {
                    EmployeeId = emp.EmployeeCode,
                    FullName = emp.FullNameaEnglish,
                    Sex = emp.Gender.ToString(),
                    HireDate = $"{emp.HiredDateAmharic} ({emp.HiredDate:MMMM dd, yyyy})",
                    AssignmentDate = $"{place.AssignmentDateAmharic} ({place.AssignmentDate:MMMM dd, yyyy})"
                };

                if (string.IsNullOrEmpty(status))
                {
                    empPerformance.Status = pendingEmployeeIds.Contains(place.EmployeeId) ? "In-Process" : "Not-Evaluated";
                }
                else if (string.Equals(status.Trim(), "Reviewed", StringComparison.OrdinalIgnoreCase))
                {
                    empPerformance.Status = "In-Process";
                }
                else if (string.Equals(status, "Not Reviewed", StringComparison.OrdinalIgnoreCase))
                {
                    empPerformance.Status = "Not-Evaluated";
                }

                if (departments.TryGetValue(place.OrgStructureId, out var departmentName))
                {
                    empPerformance.Department = departmentName;
                }
                if (businessUnits.TryGetValue(place.BusinessUnitId, out var unitName))
                {
                    empPerformance.BusinessUnit = unitName;
                }
                if (branches.TryGetValue(place.BranchId, out var branchName))
                {
                    empPerformance.Branch = branchName;
                }

                employeePerformanceList.Add(empPerformance);
            }

            return employeePerformanceList;
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}
