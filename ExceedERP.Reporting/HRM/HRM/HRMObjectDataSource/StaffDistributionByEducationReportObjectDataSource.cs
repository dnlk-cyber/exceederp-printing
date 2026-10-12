using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class StaffDistributionByEducationReportObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<StaffDistributionByEducationSummaryVM> GetStaffDistributionByEducation(object[] branchId)
        {
            var scoped = GetScopedEmployeesByBranch(branchId);
            var branchList = scoped.BranchList;
            var employeesByBranch = scoped.EmployeesByBranch;

            var result = new List<StaffDistributionByEducationSummaryVM>();

            foreach (var branch in branchList)
            {
                if (!employeesByBranch.TryGetValue(branch.BranchId, out var branchEmployees) || !branchEmployees.Any())
                {
                    continue;
                }

                var vm = new StaffDistributionByEducationSummaryVM { ProjectName = branch.Name };

                foreach (var emp in branchEmployees)
                {
                    bool isMale = emp.Gender == Gender.MALE;
                    bool isFemale = emp.Gender == Gender.FEMALE;

                    switch (emp.Status)
                    {
                        case EmployeeStatus.PERMANENT:
                            if (isMale) vm.PermanentMale++;
                            if (isFemale) vm.PermanentFemale++;
                            break;
                        case EmployeeStatus.CONTRACT:
                            if (isMale) vm.ContractMale++;
                            if (isFemale) vm.ContractFemale++;
                            break;
                        case EmployeeStatus.TEMPORARY:
                            if (isMale) vm.TemporaryMale++;
                            if (isFemale) vm.TemporaryFemale++;
                            break;
                    }
                }

                result.Add(vm);
            }

            return result.OrderBy(x => x.ProjectName).ToList();
        }

        // Dynamic, Lookup-driven education-level breakdown: one row per (Project, EducationLevel) so the
        // Crosstab report item can generate its columns straight from whatever LookUpTypes.EDUCATION_LEVEL entries exist,
        // instead of a fixed/guessed set of buckets.
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<StaffDistributionByEducationLevelCrosstabRowVM> GetStaffDistributionByEducationLevelCrosstab(object[] branchId)
        {
            var scoped = GetScopedEmployeesByBranch(branchId);
            var branchList = scoped.BranchList;
            var employeesByBranch = scoped.EmployeesByBranch;
            var allEmployeeIds = employeesByBranch.Values.SelectMany(x => x.Select(e => e.EmployeeId)).ToList();

            var educationLevels = db.Lookups
                .Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL)
                .OrderBy(x => x.Index)
                .ToList();

            const string noEducationLabel = "የትምህርት ማስረጃ የሌለው"; // No education record
            const int noEducationIndex = -1; // sorts before every real education level

            // Each employee's highest attained education level (by Lookup.Index, same ranking used elsewhere in the app)
            var highestEduLookupIdByEmployee = db.Education
                .Where(edu => allEmployeeIds.Contains(edu.EmployeeId))
                .Join(db.Lookups, edu => edu.EduLevel, lookup => lookup.LookupId,
                    (edu, lookup) => new { edu.EmployeeId, lookup.LookupId, lookup.Index })
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Index).First().LookupId);

            var result = new List<StaffDistributionByEducationLevelCrosstabRowVM>();

            foreach (var branch in branchList)
            {
                if (!employeesByBranch.TryGetValue(branch.BranchId, out var branchEmployees) || !branchEmployees.Any())
                {
                    continue;
                }

                var countsByLookupId = branchEmployees
                    .Select(e => highestEduLookupIdByEmployee.TryGetValue(e.EmployeeId, out var lookupId) ? lookupId : (int?)null)
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Emit every known education level (even with a zero count) so it always appears as a column
                foreach (var level in educationLevels)
                {
                    result.Add(new StaffDistributionByEducationLevelCrosstabRowVM
                    {
                        ProjectName = branch.Name,
                        EducationLevelName = level.Description,
                        EducationLevelIndex = level.Index,
                        Count = countsByLookupId.TryGetValue(level.LookupId, out var count) ? count : 0
                    });
                }

                result.Add(new StaffDistributionByEducationLevelCrosstabRowVM
                {
                    ProjectName = branch.Name,
                    EducationLevelName = noEducationLabel,
                    EducationLevelIndex = noEducationIndex,
                    Count = countsByLookupId.TryGetValue(null, out var noEduCount) ? noEduCount : 0
                });
            }

            return result;
        }

        // Employees in scope (active, placed, Permanent/Contract/Temporary), grouped by their current branch
        private ScopedEmployeeData GetScopedEmployeesByBranch(object[] branchId)
        {
            HashSet<int> branchIds = new HashSet<int>();
            if (branchId != null && branchId.Length > 0)
            {
                foreach (var b in branchId)
                {
                    if (b != null && int.TryParse(b.ToString(), out int bId))
                    {
                        branchIds.Add(bId);
                    }
                }
            }

            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            var placementByEmployee = placements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.First());

            var employees = db.Person_Employee
                .Where(x => x.IsActive && x.IsPlaced &&
                    (x.Status == EmployeeStatus.PERMANENT || x.Status == EmployeeStatus.CONTRACT || x.Status == EmployeeStatus.TEMPORARY))
                .ToList()
                .Where(x => placementByEmployee.ContainsKey(x.EmployeeId))
                .ToList();

            var branches = db.Branch.AsQueryable();
            if (branchIds.Count > 0)
            {
                branches = branches.Where(x => branchIds.Contains(x.BranchId));
            }
            var branchList = branches.ToList();

            var employeesByBranch = employees
                .GroupBy(e => placementByEmployee[e.EmployeeId].BranchId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return new ScopedEmployeeData { BranchList = branchList, EmployeesByBranch = employeesByBranch };
        }

        private class ScopedEmployeeData
        {
            public List<ExceedERP.Core.Domain.Common.HRM.Branch> BranchList { get; set; }
            public Dictionary<int, List<ExceedERP.Core.Domain.Common.HRM.Person_Employee>> EmployeesByBranch { get; set; }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
