using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class GenderPercentageObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenderPercentageVM> GetGenderPercentage(
            object branchIds,
            string businessUnitId,
            string orgStructureId,
            string status,
            string employementType = "",
            string age = "")
        {
            var employeesQuery = db.Person_Employee.AsQueryable();
            var placements = db.Placements.AsQueryable();

            // 1. Initial baseline restriction (matches EmployeeSummaryObjectDataSource)
            if (placements.Any())
            {
                var baseEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => baseEmpIds.Contains(x.EmployeeId));
            }

            // 2. Branch filter: progressively filter the `placements` queryable!
            List<int> branchIdList = new List<int>();
            if (branchIds != null)
            {
                if (branchIds is IEnumerable enumerable && !(branchIds is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null && int.TryParse(item.ToString(), out int id))
                            branchIdList.Add(id);
                    }
                }
                else if (int.TryParse(branchIds.ToString(), out int id))
                {
                    branchIdList.Add(id);
                }

                if (branchIdList.Any())
                {
                    placements = placements.Where(x => branchIdList.Contains(x.BranchId));
                    var branchEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                    employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
                }
            }

            // 3. Org Structure filter: progressively filter `placements`
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = placements.Where(x => x.OrgStructureId == orgId);
                var placementEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
            }

            // 4. Business Unit filter: progressively filter `placements`
            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int buId))
            {
                placements = placements.Where(p => p.BusinessUnitId == buId);
                var buEmpIds = placements.Select(p => p.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(e => buEmpIds.Contains(e.EmployeeId));
            }

            // 5. Employment Type filter
            if (!string.IsNullOrEmpty(employementType))
            {
                placements = placements.Where(x => x.EmployementType.ToString() == employementType);
                var empTypeEmpIds = placements.Select(x => x.EmployeeId).Distinct();
                employeesQuery = employeesQuery.Where(x => empTypeEmpIds.Contains(x.EmployeeId));
            }

            // 6. Status filter: check `isCurrent` ON THE NARROWED placements query!
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    var currentEmpIds = placements
                        .Where(x => x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employeesQuery = employeesQuery.Where(x => x.IsActive && currentEmpIds.Contains(x.EmployeeId));
                }
                else if (status.Equals("InActive", StringComparison.OrdinalIgnoreCase) || status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    var inactiveEmpIds = placements
                        .Where(x => !x.isCurrent)
                        .Select(x => x.EmployeeId)
                        .Distinct();

                    employeesQuery = employeesQuery.Where(x => !x.IsActive && inactiveEmpIds.Contains(x.EmployeeId));
                }
            }

            // 7. Base Scope: Require calculated Age (matches GetEmployeeSummary's x.Age.HasValue)
            var baseQuery = employeesQuery.Where(x => x.Age.HasValue);

            // 8. Age bracket filter (matches GetEmployeeSummaryByAge exactly)
            if (!string.IsNullOrEmpty(age))
            {
                switch (age)
                {
                    case "< 18":
                        baseQuery = baseQuery.Where(x => x.Age < 18);
                        break;
                    case "18-29":
                        baseQuery = baseQuery.Where(x => x.Age >= 18 && x.Age <= 29);
                        break;
                    case "30-40":
                        baseQuery = baseQuery.Where(x => x.Age >= 30 && x.Age <= 40);
                        break;
                    case "41-50":
                        baseQuery = baseQuery.Where(x => x.Age >= 41 && x.Age <= 50);
                        break;
                    case "51-60":
                        baseQuery = baseQuery.Where(x => x.Age >= 51 && x.Age <= 60);
                        break;
                    case "> 60":
                        baseQuery = baseQuery.Where(x => x.Age > 60);
                        break;
                }
            }

            // 9. Execute DB Grouping safely by integer value
            var genderCounts = baseQuery
                .GroupBy(x => (int?)x.Gender)
                .Select(g => new
                {
                    GenderValue = g.Key,
                    Count = g.Count()
                })
                .ToList();

            int maleCount = 0;
            int femaleCount = 0;
            int unspecifiedCount = 0;

            foreach (var group in genderCounts)
            {
                if (!group.GenderValue.HasValue)
                {
                    unspecifiedCount += group.Count;
                }
                else if (group.GenderValue.Value == (int)Gender.FEMALE) // 6
                {
                    femaleCount += group.Count;
                }
                else if (group.GenderValue.Value == (int)Gender.MALE) // 5
                {
                    maleCount += group.Count;
                }
                else
                {
                    unspecifiedCount += group.Count;
                }
            }

            int total = maleCount + femaleCount + unspecifiedCount;

            var result = new List<GenderPercentageVM>();

            if (maleCount > 0 || total > 0)
            {
                result.Add(new GenderPercentageVM
                {
                    Gender = HrHelper.GetLocalizedGender(Gender.MALE),
                    NumberOfEmployee = maleCount,
                    Percentage = total == 0 ? 0 : Math.Round((decimal)maleCount * 100m / total, 2)
                });
            }

            if (femaleCount > 0 || total > 0)
            {
                result.Add(new GenderPercentageVM
                {
                    Gender = HrHelper.GetLocalizedGender(Gender.FEMALE),
                    NumberOfEmployee = femaleCount,
                    Percentage = total == 0 ? 0 : Math.Round((decimal)femaleCount * 100m / total, 2)
                });
            }

            if (unspecifiedCount > 0)
            {
                result.Add(new GenderPercentageVM
                {
                    Gender = "ያልተገለጸ / Unspecified",
                    NumberOfEmployee = unspecifiedCount,
                    Percentage = total == 0 ? 0 : Math.Round((decimal)unspecifiedCount * 100m / total, 2)
                });
            }

            return result;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}