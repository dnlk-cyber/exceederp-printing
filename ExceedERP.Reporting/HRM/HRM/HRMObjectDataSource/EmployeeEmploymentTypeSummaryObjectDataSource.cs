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
            var list = GetEmployees(branchId, orgStructureIds, status);
            return list.OrderBy(x => x.SummaryName).ToList();
        }

        public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status)
        {
            var employeesQuery = _db.Person_Employee.AsQueryable();
            var placements = _db.Placements.AsQueryable();

            if (placements.Any())
            {
                var empIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            // ---  Filter by branch ---
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                placements = placements.Where(x => x.BranchId == braId);
                var branchEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
            }

            // ---  Filter by OrgStructure (supporting multiple selections via object[]) ---
            if (orgStructureIds != null && orgStructureIds.Length > 0)
            {
                var orgIds = orgStructureIds
                    .Select(o => int.TryParse(o.ToString(), out int id) ? id : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (orgIds.Any())
                {
                    placements = placements.Where(x => orgIds.Contains(x.OrgStructureId));
                    var orgEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                    employeesQuery = employeesQuery.Where(x => orgEmpIds.Contains(x.EmployeeId));
                }
            }

            // --- 3️ Active / Inactive Filter (Aligned with EmployeeSummaryObjectDataSource) ---
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    placements = placements.Where(x => x.isCurrent);
                    var activeEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                    employeesQuery = employeesQuery.Where(x => x.IsActive && activeEmpIds.Contains(x.EmployeeId));
                }
                else if (status.Equals("InActive", StringComparison.OrdinalIgnoreCase))
                {
                    placements = placements.Where(x => !x.isCurrent);
                    var inactiveEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                    employeesQuery = employeesQuery.Where(x => !x.IsActive && inactiveEmpIds.Contains(x.EmployeeId));
                }
            }

            // --- 4️ Execute query once ---
            var employeeList = employeesQuery
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Gender,
                    StatusValue = (int?)e.Status
                })
                .ToList();

            // --- 5️ Prepare all EmployeeStatus values ---
            var allStatuses = Enum.GetValues(typeof(EmployeeStatus))
                .Cast<EmployeeStatus>()
                .Where(s => s != EmployeeStatus.NOTSELECTED)
                .ToList();

            var summaries = new List<EmployeeSummary>();

            foreach (var empStatus in allStatuses)
            {
                int targetStatusInt = (int)empStatus;

                var byStatus = employeeList
                    .Where(x => x.StatusValue.HasValue && x.StatusValue.Value == targetStatusInt)
                    .ToList();

                var maleCount = byStatus.Count(x => x.Gender == Gender.MALE);
                var femaleCount = byStatus.Count(x => x.Gender == Gender.FEMALE);
                var othersCount = byStatus.Count(x => x.Gender != Gender.MALE && x.Gender != Gender.FEMALE);

                var summary = new EmployeeSummary
                {
                    Criteria = "Employment Type",
                    SummaryName = GetEnumDisplayName(empStatus),
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

    public class EmployeesVm
    {
        public int EmployeeId { get; set; }
        public Gender Gender { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; }
        public int BranchId { get; set; }
        public int OrgStructureId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
    }
}