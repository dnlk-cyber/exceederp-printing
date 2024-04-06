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
            //var employeess = from e in db.Person_Employee
            //                join p in db.Placements
            //                    on e.EmployeeId equals p.EmployeeId
            //                 //where e.IsActive && p.isCurrent 
            //                select (new
            //                {

            //                    e.EmployeeId,
            //                    e.IsActive,
            //                    e.FirstNameEnglish,
            //                    e.MiddleNameEnglish,
            //                    e.LastNameEnglish,
            //                    e.DateOfBirth,
            //                    e.HiredDate,
            //                    e.Status,
            //                    e.Gender,
            //                    e.DateOfBirthAm,
            //                    p.PlacementId,
            //                    p.BranchId,
            //                    p.isCurrent,
            //                    p.Salary,
            //                    p.EmployementType,
            //                    p.AssignmentType,
            //                    p.AssignmentDate,
            //                    p.OrgStructureId
            //                });

            // var placements = employeess.DistinctByField(x => x.EmployeeId).ToList();
            var employees = db.Person_Employee.ToList();
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
               // placements = placements.Where(x => x.BranchId == braId).ToList();
               //placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
                var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
                var placemenEmpIds = placements.Where(x=> structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
                // placements = placements.Where(x => x.OrgStructureId ==orgId).ToList();

                //placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                employees = employees.Where(x=>placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

            }

            if (status == "Active")
            {
                var empIdss = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
                placements = placements.Where(x => empIdss.Contains(x.EmployeeId)).ToList();
                //placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
                employees = employees.Where(x => x.IsActive).ToList();
               // placements = placements.Where(x => x.isCurrent).ToList();

            }
            if (status == "InActive")
            {
                 var empIdss = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
                placements = placements.Where(x => empIdss.Contains(x.EmployeeId)).ToList();
                // placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
                employees = employees.Where(x => !x.IsActive).ToList();
                //placements = placements.Where(x => !x.isCurrent).ToList();

            }

            if (sex != null)
            {
                if (sex == "MALE")
                {
                     var empIdss = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
                     placements = placements.Where(x => empIdss.Contains(x.EmployeeId)).ToList();
                     placements = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements);
                    employees = employees.Where(x => x.Gender == Gender.MALE ).ToList();

                    //placements = placements.Where(x => x.Gender == Gender.MALE).ToList();

                }
                if (sex == "FEMALE")
                {
                     var empIdss = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
                     placements = placements.Where(x => empIdss.Contains(x.EmployeeId)).ToList();
                     placements = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements);
                     employees = employees.Where(x => x.Gender == Gender.FEMALE).ToList();

                    // placements = placements.Where(x => x.Gender == Gender.FEMALE).ToList();

                }
            }

            #region Age

            var empIds = placements.Select(x => x.EmployeeId).Distinct().ToList();
           // var employees = db.Person_Employee/*.Where(x => empIds.Contains(x.EmployeeId))*/;
            placements = placements.DistinctByField(x => x.EmployeeId).ToList();

            if (age != null)
            {
                db = new ExceedDbContext();
                if (age == "< 18")
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
                    //employee.NumberOfFemaleEmployee =
                    //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    //  return list;

                }
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
                    //employee.NumberOfFemaleEmployee =
                    //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
                    //                       x.Gender == Gender.FEMALE);

                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);

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
                    //     placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                    //                           (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                    //                           x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    //return list;

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
                    //employee.NumberOfFemaleEmployee =
                    //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                    //                       x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    // return list.OrderBy(x => x.SummaryName).ToList();

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
                    //employee.NumberOfFemaleEmployee =
                    //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                    //                      (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                    //                      x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                    //                    (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    // return list;

                }
                if (age == "> 60")
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
                    //employee.NumberOfFemaleEmployee =
                    //   placements
                    //       .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    //return list.OrderByDescending(x => x.index).ToList();

                }
               
            }
            else
            {
                db = new ExceedDbContext();

                age = "< 18";

                if (age == "< 18")
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
                    //employee.NumberOfFemaleEmployee =
                    //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
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
                    //employee.NumberOfFemaleEmployee =
                    //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
                    //                       x.Gender == Gender.FEMALE);

                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
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
                    //employee.NumberOfFemaleEmployee =
                    //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
                    //                       x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
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
                    //employee.NumberOfFemaleEmployee =
                    //  placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                    //                     (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
                    //                     x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
                    //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
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
                    //employee.NumberOfFemaleEmployee =
                    //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                    //                      (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
                    //                      x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
                    //                    (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);
                    // return list;

                }
                age = "> 60";
                if (age == "> 60")
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
                    //employee.NumberOfFemaleEmployee =
                    //   placements
                    //       .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                    //employee.NumberOfMaleEmployee =
                    //    placements
                    //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                    //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    //list.Add(employee);


                }
               // return list.OrderBy(x => x.index).ToList();

            }

            #endregion

            return list.OrderBy(x => x.index).ToList();
        }
    }
}