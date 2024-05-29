using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeNationSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeNationSummary(string branchId, string orgStructureId, string nation, string status)
        {

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
                employee.Criteria = "Nation";
                employee.SummaryName = n.Description;
                employee.index = index;

                employee.NumberOfFemaleEmployee = db.Person_Employee.Count(x => femaleIds.Contains(x.EmployeeId) && x.Nation == n.LookupId.ToString());

                employee.NumberOfMaleEmployee = db.Person_Employee.Count(x => maleIds.Contains(x.EmployeeId) && x.Nation == n.LookupId.ToString());

                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);

                index++;
            }

            var noNationList = db.Person_Employee.Where(x => x.Nation == null).ToList();
            if (noNationList.Any())
            {
                employee = new EmployeeSummary();

                employee.Criteria = "Nation";
                employee.SummaryName = "ያልተሞላ";
                employee.index = index + 1;

                employee.NumberOfFemaleEmployee = db.Person_Employee.Count(x => femaleIds.Contains(x.EmployeeId) && x.Nation == null);

                employee.NumberOfMaleEmployee = db.Person_Employee.Count(x => maleIds.Contains(x.EmployeeId) && x.Nation == null);

                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);
            }

            //var lookupids = natinlist.Select(x => x.LookupId).ToList();
            //var deletednationnames = emplist.Where(x => !lookupids.Contains(Convert.ToInt32(x.Nation)) && x.Nation != null).ToList();

            //if (deletednationnames.Any())
            //{
            //    employee = new EmployeeSummary();
            //    employee.Criteria = "Nation";
            //    employee.SummaryName = "የተሳሳተ ";
            //    employee.index = index + 2;

            //    employee.NumberOfFemaleEmployee = deletednationnames.Count(x => x.Gender == Gender.FEMALE);

            //    employee.NumberOfMaleEmployee = deletednationnames.Count(x => x.Gender == Gender.MALE);
            //    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //    list.Add(employee);
            //}

            return list.OrderBy(x => x.index).ToList();
        }
    }
}