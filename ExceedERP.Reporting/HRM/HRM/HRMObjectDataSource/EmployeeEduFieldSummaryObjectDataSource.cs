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
    public class EmployeeEduFieldSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduFieldSummary(string branchId, string orgStructureId, string status, string edufield = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
        
            var eduFields = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();

            // emplist = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "");
            var employees = db.Person_Employee.ToList();

            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                // placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
                var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                // placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
                var placemenEmpIds = placements.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

            }

            if (status == "Active")
            {
                // placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
                employees = employees.Where(x => x.IsActive).ToList();

            }
            if (status == "InActive")
            {
                //placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
                employees = employees.Where(x => x.IsActive).ToList();

            }

            if (edufield != null)
            {
                int.TryParse(edufield, out int lookupId);
                eduFields = eduFields.Where(x => x.LookupId == lookupId).ToList();
            }


            if (eduFields.Any())
            {
                //var malesId = emplist.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
                //var femalesId = emplist.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

                // var malesId = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();    
                // var femalesId = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();  
                var mempIds = employees.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
                var fempIds = employees.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

                foreach (var l in eduFields)
                {
                    employee = new EmployeeSummary();
                    employee.Criteria = "Field";
                    employee.SummaryName = l.Description;

                    employee.Criteria = "Edu level";
                    employee.SummaryName = l.Description;

                    employee.NumberOfMaleEmployee = db.Education.Where(x => x.EduField == l.LookupId && mempIds.Contains(x.EmployeeId)).Count();
                    employee.NumberOfFemaleEmployee = db.Education.Where(x => x.EduField == l.LookupId && fempIds.Contains(x.EmployeeId)).Count();
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    
                    if(employee.TotalEmployee > 0)
                    {
                        list.Add(employee);
                    }                 
                }
            }

            return list;
        }
    }
}