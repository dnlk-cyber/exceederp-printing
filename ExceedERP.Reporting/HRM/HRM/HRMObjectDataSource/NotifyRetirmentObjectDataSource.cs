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
    public class NotifyRetirmentObjectDataSource : List<EmployeeRetirment>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRetirment> GetNotifyRetirmentSummary(string value, string branchId, string orgStructureId)
        {
            List<EmployeeRetirment> list = new List<EmployeeRetirment>();
            EmployeeRetirment employee = new EmployeeRetirment();
            db = new ExceedDbContext();
            List<Person_Employee> emplist;
            if (branchId != null && orgStructureId != null)
            {
                emplist = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                emplist = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                emplist = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
            }
            else
            {

                emplist = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            if (value != null)
            {
                int retirementAge = 0;
                var validator = db.Configurations.Where(x => x.Attribute == "RETIREMENT_AGE").FirstOrDefault();
                if (validator != null)
                {
                    retirementAge = Convert.ToInt32(validator.CurrentValue);



                    foreach (var emp in emplist)
                    {
                        employee = new EmployeeRetirment();
                        int x = (DateTime.Now - emp.DateOfBirth).Days/365;
                        int y = retirementAge - x;
                        if (y <= Convert.ToInt32(value))
                        {
                            employee.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            employee.IdNumber = emp.EmployeeCode;
                           // employee.Sex = emp.Gender;
                            if (emp.Gender == Gender.MALE)
                            {
                                employee.Sex = "ወንድ";
                            }
                            else
                            {
                                employee.Sex = "ሴት";
                            }
                            //decimal remday = x * 365 / 30;
                            //string[] remaings = remday.ToString().Split('.');
                            //if (remaings.Length == 2)
                            //{
                            //employee.RemainingDays = remday.ToString() + ">>" + remaings[0] + "Month " + remaings[1] + "Days";
                            var retireYear = emp.DateOfBirth.AddYears(60);
                                employee.RemainingDays = retireYear.Subtract(DateTime.Today).TotalDays.ToString();
                            //}
                            //if (remaings.Length == 3)
                            //{
                            //    employee.RemainingDays = remday.ToString() + ">>" + remaings[0] + "Year " + remaings[1] +
                            //                             "Month " + remaings[2] +
                            //                             "Days";
                            //}
                            //if (remaings.Length == 1)
                            //{
                            //    employee.RemainingDays = remday.ToString() + ">>" + remaings[0] + "Days ";
                            //}
                            DateTime date = emp .DateOfBirth.AddYears(retirementAge);

                            employee.RetirmentDate = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day,date.Month,date.Year);
                            employee.BirthDate = emp.DateOfBirthAm;
                            list.Add(employee);
                        }
                    }

                }
            }
            return list;
        }
    }
}