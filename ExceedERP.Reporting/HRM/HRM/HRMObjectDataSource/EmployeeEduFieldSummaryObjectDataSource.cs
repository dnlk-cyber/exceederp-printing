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
    public class EmployeeEduFieldSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduFieldSummary(string branchId, string orgStructureId, string status, string edufield = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
            

            #region EduLevel
            List<Person_Employee> emplist;
           
            emplist = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "");
            if (status == "Active")
            {
                emplist = emplist.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                emplist = emplist.Where(x => !x.IsActive).ToList();
            }
            var eduFields = db.Lookups.Where( x=> x.LookupType ==LookUpTypes.EDUCATION_FIELD ).ToList();
            if (edufield != null)
            {
                eduFields = eduFields.Where(x => x.LookupId.ToString() == edufield).ToList();
            }

            if (eduFields.Any())
                {
                    foreach (var l in eduFields)
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = "Field";
                        employee.SummaryName = l.Description;

                        foreach (var e in emplist)
                        {
                            db = new ExceedDbContext();

                            var lists = db.Education.Where(x => x.EduField == l.LookupId && x.EmployeeId == e.EmployeeId ).ToList();
                            if (lists.Any())
                            {



                                if (e.Gender == Gender.FEMALE)
                                    employee.NumberOfFemaleEmployee += 1;
                                else
                                    employee.NumberOfMaleEmployee += 1;
                                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;


                            }
                        }
                       

                       
                            //var lookup = lookups.FirstOrDefault(x => x.LookupId.ToString() == edufield);
                            //if (lookup != null)
                            //{
                            //    employee.Field = lookup.Description;
                            //}
                        
                        list.Add(employee);
                    }
                }

 return list;

            

            #endregion



           
        }
    }
}