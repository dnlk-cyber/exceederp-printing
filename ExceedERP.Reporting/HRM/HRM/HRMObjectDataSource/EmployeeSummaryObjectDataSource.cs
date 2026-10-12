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
    public class EmployeeSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeSummary(string branchId, string orgStructureId, string sex, string status,string employementType, string age = "")
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
            else if (status == "InActive")
            {
                var empIds = placements.Where(x => !x.isCurrent)
                                       .Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => !x.IsActive && empIds.Contains(x.EmployeeId));
            }

            if (!string.IsNullOrEmpty(employementType)) {
                var empIds = placements.Where(x => x.EmployementType.ToString() == employementType)
                                       .Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));
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
            var summary = new EmployeeSummary
            {
                Criteria = CustomExpression.CustomExpression.GetStringOverride("Age"),
                SummaryName = ageRange
            };

            // Filter to only records where Age has been calculated and assigned
            var query = employeesQuery.Where(x => x.Age.HasValue);

            // 1. Slice the records into the requested age bracket
            IQueryable<Person_Employee> rangeQuery;

            switch (ageRange)
            {
                case "< 18":
                    rangeQuery = query.Where(x => x.Age < 18);
                    break;

                case "18-29":
                    rangeQuery = query.Where(x => x.Age >= 18 && x.Age <= 29);
                    break;

                case "30-40":
                    rangeQuery = query.Where(x => x.Age >= 30 && x.Age <= 40);
                    break;

                case "41-50":
                    rangeQuery = query.Where(x => x.Age >= 41 && x.Age <= 50);
                    break;

                case "51-60":
                    rangeQuery = query.Where(x => x.Age >= 51 && x.Age <= 60);
                    break;

                case "> 60":
                    rangeQuery = query.Where(x => x.Age > 60);
                    break;

                default:
                    rangeQuery = query.Where(x => false);
                    break;
            }

            // 2. Project the underlying int/nullable int value to avoid casting exceptions in SQL/LINQ
            var genderCounts = rangeQuery
                .GroupBy(x => (int?)x.Gender)
                .Select(g => new
                {
                    GenderValue = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // 3. Map strictly by enum raw integer constants (5 = MALE, 6 = FEMALE)
            foreach (var group in genderCounts)
            {
                if (!group.GenderValue.HasValue)
                {
                    // Database value is NULL
                    summary.NumberOfUnspecifiedGenderEmployee += group.Count;
                }
                else if (group.GenderValue.Value == (int)Gender.FEMALE) // 6
                {
                    summary.NumberOfFemaleEmployee += group.Count;
                }
                else if (group.GenderValue.Value == (int)Gender.MALE) // 5
                {
                    summary.NumberOfMaleEmployee += group.Count;
                }
                else
                {
                    // Any invalid or unexpected numeric value (e.g. 0, 1, 2, 99)
                    summary.NumberOfUnspecifiedGenderEmployee += group.Count;
                }
            }

            // 4. Calculate total so all rows are accounted for
            summary.TotalEmployee = summary.NumberOfFemaleEmployee
                                  + summary.NumberOfMaleEmployee
                                  + summary.NumberOfOtherEmployee
                                  + summary.NumberOfUnspecifiedGenderEmployee;

            return summary;
        }

        //private EmployeeSummary GetEmployeeSummaryByAge(IQueryable<Person_Employee> employeesQuery, string ageRange)
        //{
        //    var employee = new EmployeeSummary
        //    {
        //        Criteria = CustomExpression.CustomExpression.GetStringOverride("Age"),
        //        SummaryName = ageRange
        //    };
        //    switch (ageRange)
        //    {
        //        case "< 18":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) < 18 && x.Gender == Gender.MALE);
        //            break;

        //        case "18-29":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 18 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 29 && x.Gender == Gender.MALE);
        //            break;

        //        case "30-40":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 30 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 40 && x.Gender == Gender.MALE);
        //            break;

        //        case "41-50":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 41 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 50 && x.Gender == Gender.MALE);
        //            break;

        //        case "51-60":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= 51 && (DateTime.Now.Year - x.DateOfBirth.Year) <= 60 && x.Gender == Gender.MALE);
        //            break;

        //        case "> 60":
        //            employee.NumberOfFemaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.FEMALE);
        //            employee.NumberOfMaleEmployee = employeesQuery.Count(x => (DateTime.Now.Year - x.DateOfBirth.Year) > 60 && x.Gender == Gender.MALE);
        //            break;
        //    }

        //    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
        //    return employee;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}