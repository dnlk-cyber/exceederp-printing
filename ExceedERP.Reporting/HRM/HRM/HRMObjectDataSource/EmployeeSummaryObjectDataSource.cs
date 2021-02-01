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
    public class EmployeeSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeSummary(string branchId, string orgStructureId, string sex, string status, string age = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
            List<Person_Employee> emplist = new List<Person_Employee>();


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

            if (sex != null)
            {
                if (sex == "MALE")
                {
                    emplist = emplist.Where(x => x.Gender == Gender.MALE).ToList();
                }
                if (sex == "FEMALE")
                {
                    emplist = emplist.Where(x => x.Gender == Gender.FEMALE).ToList();
                }
            }
            #region Age

            if (age != null)
            {
                db = new ExceedDbContext();

                if (age == "18-29")
                {


                    if (emplist.Any())
                    {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 1;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 20 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 20 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list;
                    }
                }
                if (age == "30-40")
                {


                    if (emplist.Any())
                    {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 2;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list;
                    }
                }
                if (age == "41-50")
                {

                    if (emplist.Any())
                    {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 3;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list.OrderBy(x => x.SummaryName).ToList();
                    }
                }
                if (age == "51-60")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 4;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                            (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                    }
                }
                if (age == "More than 60")
                {

                    if (emplist.Any())
                    {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 5;
                        employee.NumberOfFemaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list.OrderByDescending(x => x.index).ToList();
                    }
                }
            }
            else
            {
                db = new ExceedDbContext();

                age = "Less than 18";

                if (age == "Less than 18")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 0;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        //  return list;
                    }
                }

                age = "18-29";

                if (age == "18-29")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 1;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
                                               x.Gender == Gender.FEMALE);

                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        //  return list;
                    }
                }
                age = "30-40";
                if (age == "30-40")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.index = 2;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                    }
                }
                age = "41-50";
                if (age == "41-50")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.index = 3;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                    }
                }
                age = "51-60";
                if (age == "51-60")
                {

                    if (emplist.Any())
                    {
                        employee = new EmployeeSummary();
                        employee.index = 4;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            emplist.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                            (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                    }
                }
                age = "More than 60";
                if (age == "More than 60")
                {
                    employee = new EmployeeSummary();

                    if (emplist.Any())
                    {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 5;
                        employee.NumberOfFemaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            emplist
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);

                    }
                }
                return list.OrderByDescending(x => x.index).ToList();

            }

            #endregion



            return list.OrderByDescending(x => x.index).ToList();
        }
    }
}