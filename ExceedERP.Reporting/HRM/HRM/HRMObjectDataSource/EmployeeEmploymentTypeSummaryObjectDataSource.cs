using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class EmployeeEmploymentTypeSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId, string orgStructureId, string status, string source, string empStatus = "")
        {
            var list = GetEmployees(branchId, orgStructureId, status);

            return list.OrderBy(x => x.SummaryName).ToList();
        }

        public List<EmployeeSummary> GetEmployees(string branchId, string orgStructureId, string status)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();

            var placementsss = db.Placements.ToList();

            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
                placementsss = placementsss.Where(x => x.BranchId == braId).ToList();
            }
            if (!string.IsNullOrEmpty(orgStructureId))
            {
                int.TryParse(orgStructureId, out int orgId);
                placementsss = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placementsss, orgId);
            }

            if (status == "Active")
            {
                placementsss = placementsss.Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (status == "InActive")
            {
                placementsss = placementsss.Where(x => !x.isCurrent).DistinctByField(x => x.EmployeeId).DistinctByField(x => x.EmployeeId).ToList();
            }

            var males = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
            var females = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

            var allTypes = Enum.GetValues(typeof(EmployeeStatus))
                           .Cast<EmployeeStatus>()
                           .ToList().Select(s => new { Text = s.ToString(), Value = s.ToString() });

            foreach (var a in allTypes)
            {
                var emplistByStatus = placementsss.Where(x => x.EmployementType.ToString() == a.Value).DistinctByField(x => x.EmployeeId).ToList();

                var employee = new EmployeeSummary();

                employee.Criteria = "Employement Type";

                employee.SummaryName = GetStatus(a.Value.ToString());
                employee.NumberOfFemaleEmployee = emplistByStatus.Where(x => males.Contains(x.EmployeeId)).Count();
                employee.NumberOfMaleEmployee = emplistByStatus.Where(x => females.Contains(x.EmployeeId)).Count();
                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);

            }

            return list;
        }
        public string GetStatus(string empStatus)
        {
            switch (empStatus)
            {
                case "PERMANENT":
                    return "ቋሚ";

                case "CONTRACT":
                    return "ጊዜያዊ";

                case "PROBATION":
                    return "የሙከራ ጊዜ";
                case "FREELANCER":
                    return "ፍሪላንሰር";
                case "NO_STATUS":
                    return "የማይታወቅ";


            }
            return empStatus;
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