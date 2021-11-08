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
          
            var placements = db.Placements.ToList();

            if(branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }            

            if(status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
            }

            if (sex != null)
            {
                if (sex == "MALE")
                {
                    placements = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements);
                }
                if (sex == "FEMALE")
                {
                    placements = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements);
                }
            }

            #region Age

            var empIds = placements.Select(x => x.EmployeeId).Distinct().ToList();
            var employees = db.Person_Employee.Where(x => empIds.Contains(x.EmployeeId));

            if (age != null)
            {
                db = new ExceedDbContext();

                if (age == "18-29")
                {
                   
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 1;                      

                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 20 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 20 && x.Gender == Gender.MALE);

                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                        list.Add(employee);

                        return list;
                  
                }
                if (age == "30-40")
                {
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 2;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list;
                    
                }
                if (age == "41-50")
                {
                    
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 3;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list.OrderBy(x => x.SummaryName).ToList();
                   
                }
                if (age == "51-60")
                {

                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 4;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                            (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                   
                }
                if (age == "More than 60")
                {

                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 5;
                        employee.NumberOfFemaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        return list.OrderByDescending(x => x.index).ToList();
                   
                }
            }
            else
            {
                db = new ExceedDbContext();

                age = "Less than 18";

                if (age == "Less than 18")
                {
                    
                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 0;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        //  return list;
                   
                }

                age = "18-29";

                if (age == "18-29")
                {

                   
                        employee = new EmployeeSummary();
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 1;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
                                               x.Gender == Gender.FEMALE);

                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        //  return list;
                   
                }
                age = "30-40";
                if (age == "30-40")
                {
                    
                        employee = new EmployeeSummary();
                        employee.index = 2;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                   
                }
                age = "41-50";
                if (age == "41-50")
                {
                    
                        employee = new EmployeeSummary();
                        employee.index = 3;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                  
                }
                age = "51-60";
                if (age == "51-60")
                {
                    
                        employee = new EmployeeSummary();
                        employee.index = 4;
                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.NumberOfFemaleEmployee =
                            employees.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                               (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                                               x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                                            (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);
                        // return list;
                   
                }
                age = "More than 60";
                if (age == "More than 60")
                {
                    employee = new EmployeeSummary();

                        employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
                        employee.SummaryName = age;
                        employee.index = 5;
                        employee.NumberOfFemaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee =
                            employees
                                .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        list.Add(employee);

                   
                }
                return list.OrderByDescending(x => x.index).ToList();

            }

            #endregion



            return list.OrderByDescending(x => x.index).ToList();
        }
    }
}