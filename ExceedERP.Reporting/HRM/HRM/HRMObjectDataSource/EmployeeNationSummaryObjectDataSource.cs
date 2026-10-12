using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Localization;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeNationSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeNationSummary(string branchId, string orgStructureId, string nation, string status)
        {
            string cultureName = GlobalHelper.GetCurrentCulture();
            var employees = db.Person_Employee.ToList();
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.NATION).ToList();

            int index = 0;

            list = new List<EmployeeSummary>();
            employee = new EmployeeSummary();

            List<Person_Employee> emplist;
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
                //placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
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
                // placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
                employees = employees.Where(x => !x.IsActive).ToList();

            }
            if (nation != null)
            {
                int.TryParse(nation, out int lookupId);
                lookups = lookups.Where(x => x.LookupId == lookupId).ToList();
            }

            var maleIds = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x=>x.EmployeeId).ToList();
            var femaleIds = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();

            foreach (var n in lookups)
            {
                employee = new EmployeeSummary();
                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                {
                    employee.Criteria = "ብሄር";
                }
                if (cultureName.ToLower() == "om")
                {
                    employee.Criteria = "Saba";
                }
                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                {
                    employee.Criteria = "Nation";
                }
                employee.SummaryName = n.Description;
                employee.index = index;

                //employee.NumberOfFemaleEmployee = db.Person_Employee.Count(x => femaleIds.Contains(x.EmployeeId) && x.Nation == n.LookupId.ToString());

                //employee.NumberOfMaleEmployee = db.Person_Employee.Count(x => maleIds.Contains(x.EmployeeId) && x.Nation == n.LookupId.ToString());
                employee.NumberOfFemaleEmployee =employees.Count(x => x.Gender==Gender.FEMALE  && x.Nation == n.LookupId.ToString());

                employee.NumberOfMaleEmployee = employees.Count(x => x.Gender == Gender.MALE && x.Nation == n.LookupId.ToString());

                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);

                index++;
            }

            var noNationList = employees.Where(x => x.Nation == null).ToList();
            if (noNationList.Any())
            {
                employee = new EmployeeSummary();

                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                {
                    employee.Criteria = "ብሄር";
                    employee.SummaryName = "ያልተሞላ";

                }
                if (cultureName.ToLower() == "om")
                {
                    employee.Criteria = "Saba";
                    employee.SummaryName = "Hin guutamne";
                }
                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                {
                    employee.Criteria = "Nation";
                    employee.SummaryName = "Unfilled";

                }
                employee.index = index + 1;

                employee.NumberOfFemaleEmployee = employees.Count(x => x.Gender == Gender.FEMALE && x.Nation == null);

                employee.NumberOfMaleEmployee = employees.Count(x => x.Gender == Gender.MALE && x.Nation == null);

                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);
            }
            
            return list.OrderBy(x => x.index).ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}