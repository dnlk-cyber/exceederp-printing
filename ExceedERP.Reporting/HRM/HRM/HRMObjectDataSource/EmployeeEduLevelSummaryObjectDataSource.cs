using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
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
            db = new ExceedDbContext();




            #region EduLevel
            List<Person_Employee> emplist;

            emplist = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "");

            if(status == "Active")
            {
                emplist = emplist.Where(x=>x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                emplist = emplist.Where(x => !x.IsActive).ToList();
            }

            var edulevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
            if (eduLevel != null)
            {
                edulevels = edulevels.Where(x => x.LookupId.ToString() == eduLevel).ToList();
            }

            if (edulevels.Any())
            {
                foreach (var l in edulevels)
                {

                    employee = new EmployeeSummary();
                    employee.Criteria = "Edu level";
                    employee.SummaryName = l.Description;
                    foreach (var e in emplist)
                    {
                        db = new ExceedDbContext();
                        var lists = db.Education.Where(x => x.EduLevel == l.LookupId && x.EmployeeId == e.EmployeeId).ToList();

                        if (lists.Any())
                        {



                            if (e.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            else
                            {
                                employee.NumberOfMaleEmployee += 1;
                            }

                            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                        }
                    }


                    list.Add(employee);

                }
            }





            #endregion




            return list;
        }
    }
}