using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEduLevelSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduLevelSummary(string branchId, string orgStructureId, string status, string eduLevel = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();

            #region EduLevel
            // List<Person_Employee> emplist = db.Person_Employee.ToList(); ;

            //   emplist = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "");

            var edulevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();

            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                
            }

            if (status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
            }

            if (eduLevel != null)
            {
                int.TryParse(eduLevel, out int lookupId);
                edulevels = edulevels.Where(x => x.LookupId == lookupId).ToList();
            }

            if (edulevels.Any())
            {
                var malesId = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList(); //emplist.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
                var femalesId = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList(); //emplist.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

                foreach (var l in edulevels)
                {
                    employee = new EmployeeSummary();

                    employee.Criteria = "Edu level";
                    employee.SummaryName = l.Description;

                    employee.NumberOfMaleEmployee = db.Education.Where(x => x.EduLevel == l.LookupId && malesId.Contains(x.EmployeeId)).Count();
                    employee.NumberOfFemaleEmployee = db.Education.Where(x => x.EduLevel == l.LookupId && femalesId.Contains(x.EmployeeId)).Count();
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    if (employee.TotalEmployee > 0)
                    {
                        list.Add(employee);
                    }


                }
            }

            #endregion

            return list;
        }
    }
}