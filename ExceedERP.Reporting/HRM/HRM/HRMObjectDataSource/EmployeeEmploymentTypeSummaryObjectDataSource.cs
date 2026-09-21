using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEmploymentTypeSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId, object[] orgStructureIds, string status, string source, string empStatus = "")
        {
            // Pass empStatus down to the query method
            var list = GetEmployees(branchId, orgStructureIds, status, empStatus);
            return list.OrderBy(x => x.SummaryName).ToList();
        }

        public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status, string empStatus = "")
        {
            IQueryable<Placement> placements = _db.Placements;
            IQueryable<Person_Employee> employees = _db.Person_Employee.Where(x => x.IsActive);

            // Filter by branch
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branch))
            {
                var empIds = placements
                    .Where(x => x.BranchId == branch)
                    .Select(x => x.EmployeeId)
                    .Distinct();

                employees = employees.Where(x => empIds.Contains(x.EmployeeId));
            }

            // Filter by OrgStructure
            if (orgStructureIds != null && orgStructureIds.Length > 0)
            {
                var orgIds = orgStructureIds
                    .Select(o => int.TryParse(o?.ToString(), out int id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (orgIds.Any())
                {
                    var empIds = placements
                        .Where(x => orgIds.Contains(x.OrgStructureId))
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => empIds.Contains(x.EmployeeId));
                }
            }

            // Active / Inactive Filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    var activeEmpIds = placements
                        .Where(x => x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => activeEmpIds.Contains(x.EmployeeId) && x.IsActive);
                }
                else if (status.Equals("InActive", StringComparison.OrdinalIgnoreCase))
                {
                    var inactiveEmpIds = placements
                        .Where(x => !x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employees = employees.Where(x => inactiveEmpIds.Contains(x.EmployeeId) && !x.IsActive);
                }
            }

            // Execute query once
            var employeeList = employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Gender,
                    e.Status
                })
                .ToList();

            // Prepare EmployeeStatus values
            var allStatuses = Enum.GetValues(typeof(EmployeeStatus))
                .Cast<EmployeeStatus>()
                .Where(s => s != EmployeeStatus.NOTSELECTED)
                .ToList();

            // Filter by Employment Type / Status if a specific option is chosen
            if (!string.IsNullOrEmpty(empStatus) && !empStatus.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                allStatuses = allStatuses.Where(s =>
                    s.ToString().Equals(empStatus, StringComparison.OrdinalIgnoreCase) ||
                    GetEnumDisplayName(s).Equals(empStatus, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var summaries = new List<EmployeeSummary>();

            foreach (var statusItem in allStatuses)
            {
                var byStatus = employeeList
                    .Where(x => x.Status == statusItem)
                    .ToList();

                var maleCount = byStatus.Count(x => x.Gender == Gender.MALE);
                var femaleCount = byStatus.Count(x => x.Gender == Gender.FEMALE);
                var othersCount = byStatus.Count(x => x.Gender != Gender.MALE && x.Gender != Gender.FEMALE);

                var summary = new EmployeeSummary
                {
                    Criteria = "Employment Type",
                    SummaryName = GetEnumDisplayName(statusItem),
                    NumberOfMaleEmployee = maleCount,
                    NumberOfFemaleEmployee = femaleCount,
                    NumberOfOtherEmployee = othersCount,
                    TotalEmployee = maleCount + femaleCount + othersCount
                };

                summaries.Add(summary);
            }

            return summaries;
        }

        public string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                         .Cast<DisplayAttribute>()
                                         .FirstOrDefault();

            return displayAttribute?.GetName() ?? value.ToString();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}