using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
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

            var list = new List<EmployeeSummary>();

            // Start with a queryable collection instead of loading everything into memory
            var employeesQuery = db.Person_Employee.AsQueryable();
            var placements = db.Placements.AsQueryable();
            if (placements.Any())
            {
                var empIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));

            }
            // Apply branch filtering
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                placements = placements.Where(x => x.BranchId == braId);
                var branchEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));

            }

            // Apply organizational structure filtering
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {

        
                placements = placements.Where(x =>x.OrgStructureId == orgId);
                var placementEmpIds = placements
                                        .Where(x => x.OrgStructureId == orgId)
                                        .Select(x => x.EmployeeId).Distinct();

                employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
            }

            // Apply status filtering
            if (status == "Active") {
                var empIds = placements.Where(x => x.isCurrent)
                                       .Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => x.IsActive && empIds.Contains(x.EmployeeId));

            }
            if (status == "InActive") {
                var empIds = placements.Where(x => !x.isCurrent)
                                       .Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => !x.IsActive && empIds.Contains(x.EmployeeId));
            }

            // Apply gender filtering
            if (!string.IsNullOrEmpty(sex))
            {
                if (sex == "MALE") employeesQuery = employeesQuery.Where(x => x.Gender == Gender.MALE);
                if (sex == "FEMALE") employeesQuery = employeesQuery.Where(x => x.Gender == Gender.FEMALE);
            }

            // Apply age filtering
            if (!string.IsNullOrEmpty(age))
            {
                var employeeSummary = GetEmployeeSummaryByAge(employeesQuery, age);
                list.Add(employeeSummary);
            }
            else
            {
                string[] ageRanges = { "< 18", "18-29", "30-40", "41-50", "51-60", "> 60" };
                for (int i = 0; i < ageRanges.Length; i++)
                {
                    var employeeSummary = GetEmployeeSummaryByAge(employeesQuery, ageRanges[i]);
                    employeeSummary.index = i;
                    list.Add(employeeSummary);
                }
            }

            return list.OrderBy(x => x.index).ToList();
        }

        private EmployeeSummary GetEmployeeSummaryByAge(IQueryable<Person_Employee> employeesQuery, string ageRange)
        {
            var employee = new EmployeeSummary
            {
                Criteria = CustomExpression.CustomExpression.GetStringOverride("Age"),
                SummaryName = ageRange
            };
            switch (ageRange)
            {
                case "< 18":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
                    break;

                case "18-29":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
                    break;

                case "30-40":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
                    break;

                case "41-50":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
                    break;

                case "51-60":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
                    break;

                case "> 60":
                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
                    break;
            }

            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
            return employee;
        }
        //public List<EmployeeSummary> GetEmployeeSummary(string branchId, string orgStructureId, string sex, string status, string age = "")
        //{
        //    List<EmployeeSummary> list = new List<EmployeeSummary>();
        //    EmployeeSummary employee = new EmployeeSummary();

        //    // Start with a queryable collection instead of loading everything into memory
        //    var employeesQuery = db.Person_Employee.AsQueryable();

        //    // Apply branch filtering
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {
        //        var branchEmpIds = db.Placements
        //                             .Where(x => x.BranchId == braId)
        //                             .Select(x => x.EmployeeId)
        //                             .Distinct();
        //        employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
        //    }

        //    // Apply organizational structure filtering
        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures
        //                             .Where(x => x.Parent == orgId)
        //                             .Select(x => x.OrgStructureId)
        //                             .ToList();

        //        var placementEmpIds = db.Placements
        //                                .Where(x => structureIds.Contains(x.OrgStructureId))
        //                                .Select(x => x.EmployeeId)
        //                                .Distinct();

        //        employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
        //    }

        //    // Apply status filtering
        //    if (status == "Active") employeesQuery = employeesQuery.Where(x => x.IsActive);
        //    if (status == "InActive") employeesQuery = employeesQuery.Where(x => !x.IsActive);

        //    // Apply gender filtering
        //    if (!string.IsNullOrEmpty(sex))
        //    {
        //        if (sex == "MALE") employeesQuery = employeesQuery.Where(x => x.Gender == Gender.MALE);
        //        if (sex == "FEMALE") employeesQuery = employeesQuery.Where(x => x.Gender == Gender.FEMALE);
        //    }

        //    #region Age

        //    // var empIds = placements.Select(x => x.EmployeeId).Distinct().ToList();
        //    //// var employees = db.Person_Employee/*.Where(x => empIds.Contains(x.EmployeeId))*/;
        //    // placements = placements.DistinctByField(x => x.EmployeeId).ToList();

        //    if (!string.IsNullOrEmpty(age))
        //    {
        //        db = new ExceedDbContext();
        //        if (age == "< 18")
        //        {

        //            employee = new EmployeeSummary();
        //            employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //            employee.SummaryName = age;
        //            employee.index = 0;
        //            employee.NumberOfFemaleEmployee =
        //                employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

        //            employee.NumberOfMaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //employee.NumberOfFemaleEmployee =
        //            //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);

        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);
        //            //  return list;

        //        }
        //        if (age == "18-29")
        //        {

        //            employee = new EmployeeSummary();
        //            employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //            employee.SummaryName = age;
        //            employee.index = 1;
        //            employee.NumberOfFemaleEmployee =
        //                employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
        //                                   x.Gender == Gender.FEMALE);

        //            employee.NumberOfMaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //employee.NumberOfFemaleEmployee =
        //            //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
        //            //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 &&
        //            //                       x.Gender == Gender.FEMALE);

        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 &&
        //            //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);

        //        }
        //        if (age == "30-40")
        //        {
        //                employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //                employee.SummaryName = age;
        //                employee.index = 2;
        //                employee.NumberOfFemaleEmployee =
        //                 employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
        //                                        (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
        //                                        x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //     placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
        //            //                           (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 &&
        //            //                           x.Gender == Gender.FEMALE);
        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 &&
        //            //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);
        //            //return list;

        //        }
        //        if (age == "41-50")
        //        {

        //                employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //                employee.SummaryName = age;
        //                employee.index = 3;
        //            employee.NumberOfFemaleEmployee =
        //                employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
        //                                   x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //employee.NumberOfFemaleEmployee =
        //            //    placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
        //            //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 &&
        //            //                       x.Gender == Gender.FEMALE);
        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 &&
        //            //                       (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);
        //            // return list.OrderBy(x => x.SummaryName).ToList();

        //        }
        //        if (age == "51-60")
        //        {

        //                employee = new EmployeeSummary();
        //                employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //                employee.SummaryName = age;
        //                employee.index = 4;
        //            employee.NumberOfFemaleEmployee =
        //                employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
        //                                   (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
        //                                   x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee =
        //                employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
        //                                (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //employee.NumberOfFemaleEmployee =
        //            //   placements.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
        //            //                      (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 &&
        //            //                      x.Gender == Gender.FEMALE);
        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 &&
        //            //                    (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);
        //            // return list;

        //        }
        //        if (age == "> 60")
        //        {

        //                employee.Criteria = CustomExpression.CustomExpression.GetStringOverride("Age");
        //                employee.SummaryName = age;
        //                employee.index = 5;
        //            employee.NumberOfFemaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee =
        //                employeesQuery
        //                    .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //            //employee.NumberOfFemaleEmployee =
        //            //   placements
        //            //       .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
        //            //employee.NumberOfMaleEmployee =
        //            //    placements
        //            //        .Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
        //            //employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            //list.Add(employee);
        //            //return list.OrderByDescending(x => x.index).ToList();

        //        }

        //    }
        //    else
        //    {
        //        db = new ExceedDbContext();



        //        string[] ageRanges = { "< 18", "18-29", "30-40", "41-50", "51-60", "> 60" };

        //        for (int i = 0; i < ageRanges.Length; i++)
        //        {
        //            var ageRange = ageRanges[i];
        //             employee = new EmployeeSummary
        //            {
        //                Criteria = CustomExpression.CustomExpression.GetStringOverride("Age"),
        //                SummaryName = ageRange,
        //                index = i
        //            };

        //            switch (ageRange)
        //            {
        //                case "< 18":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
        //                    break;

        //                case "18-29":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
        //                    break;

        //                case "30-40":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
        //                    break;

        //                case "41-50":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
        //                    break;

        //                case "51-60":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
        //                    break;

        //                case "> 60":
        //                    employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
        //                    employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
        //                    break;
        //            }

        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //            list.Add(employee);
        //        }


        //        // list now contains the summarized data

        //        // return list.OrderBy(x => x.index).ToList();

        //    }

        //    #endregion

        //    return list.OrderBy(x => x.index).ToList();
        //}

    }
}