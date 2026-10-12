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
    public class RecruitmentObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetRecruitmentSummary(DateTime from, DateTime to, string branchId, string orgStructureId)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);

            List<Person_Employee> recruitment;
            if (branchId != null && orgStructureId != null)
            {
                recruitment = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                recruitment = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                recruitment = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
            }
            else
            {

                recruitment = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            employee = new EmployeeSummary();
            if (recruitment.Count() > 0)
            {
                foreach (var r in recruitment)
                {

                    db = new ExceedDbContext();
                    // employee.Criteria = "Job Title";
                    //  var regDate= Convert.ToDateTime((r.RegistrationDate.ToString("yy-MM-dd"))).Date;
                    DateTime regDate = new DateTime(r.HiredDate.Year, r.HiredDate.Month,
                        r.HiredDate.Day);
                    if (regDate >= dateOne && regDate <= dateTow)
                    {
                        if (r.Gender == Gender.FEMALE)
                        {
                            employee.NumberOfFemaleEmployee += 1;

                        }
                        if (r.Gender == Gender.MALE)
                        {
                            employee.NumberOfMaleEmployee += 1;

                        }



                    }
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                }
                list.Add(employee);
            }
            return list;



        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}