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
            var lookups = db.Lookups.ToList();

            int index = 0;

            list = new List<EmployeeSummary>();
            employee = new EmployeeSummary();

            #region Nation
            List<Person_Employee> emplist;
            if (branchId != null && orgStructureId != null)
            {
                emplist = EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                emplist = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                emplist = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {

                emplist = db.Person_Employee.ToList();

            }
            if(status == "Active")
            {
                emplist = emplist.Where(x=>x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                emplist = emplist.Where(x => !x.IsActive).ToList();
            }
            if (nation != null)
            {
                employee = new EmployeeSummary();
                db = new ExceedDbContext();
                employee.Criteria = "Nation";
                employee.SummaryName =
                    lookups.FirstOrDefault(x => x.LookupId.ToString() == nation)?.Description;
                if (emplist.Any())
                {

                    employee.NumberOfFemaleEmployee = emplist.Count(x => x.Gender == Gender.FEMALE && x.Nation == nation);
                    employee.NumberOfMaleEmployee = emplist.Count(x => x.Gender == Gender.MALE && x.Nation == nation);
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    list.Add(employee);
                    return list;
                }
            }
            else
            {
                db = new ExceedDbContext();


                var natinlist = db.Lookups.Where(x => x.LookupType == LookUpTypes.NATION).ToList();
                if (natinlist.Any())
                {
                    foreach (var n in natinlist)
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = "Nation";
                        employee.SummaryName = n.Description;
                        employee.index = index;
                        if (emplist.Any())
                        {


                            employee.NumberOfFemaleEmployee =
                                emplist
                                    .Count(x => x.Gender == Gender.FEMALE && x.Nation == n.LookupId.ToString());

                            employee.NumberOfMaleEmployee =
                                emplist.Count(x => x.Gender == Gender.MALE && x.Nation == n.LookupId.ToString());
                            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;


                        }
                        list.Add(employee);

                        index++;
                    }
                    var noNationList = emplist.Where(x=>x.Nation == null).ToList();
                    if(noNationList.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = "Nation";
                        employee.SummaryName = "ያልተሞላ";
                        employee.index = index + 1;

                        employee.NumberOfFemaleEmployee =
                             emplist
                                 .Count(x => x.Gender == Gender.FEMALE && x.Nation == null);

                        employee.NumberOfMaleEmployee =
                            emplist.Count(x => x.Gender == Gender.MALE && x.Nation == null);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                        list.Add(employee);
                    }
                    var lookupids = natinlist.Select(x=>x.LookupId).ToList();
                    var deletednationnames = emplist.Where(x=>!lookupids.Contains(Convert.ToInt32(x.Nation)) && x.Nation != null).ToList();
                   
                    if (deletednationnames.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = "Nation";
                        employee.SummaryName = "የተሳሳተ ";
                        employee.index = index + 2;

                        employee.NumberOfFemaleEmployee =
                             deletednationnames
                                 .Count(x => x.Gender == Gender.FEMALE);

                        employee.NumberOfMaleEmployee =
                            deletednationnames.Count(x => x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                        list.Add(employee);
                    }
                }
            }

            #endregion

            return list.OrderBy(x=>x.index).ToList();
        }
    }
}