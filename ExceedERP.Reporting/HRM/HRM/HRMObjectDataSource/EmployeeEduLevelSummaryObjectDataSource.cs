using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEduLevelSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduLevelSummary(string branchId, string orgStructureId, string status, string eduLevel = "")
        {
            var list = new List<EmployeeSummary>();
            bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et";
            bool isOromo = cultureName.ToLower() == "om";
            // Fetch the educational levels
            var edulevelsQuery = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL);
            if (!string.IsNullOrEmpty(eduLevel) && int.TryParse(eduLevel, out int lookupId))
            {
                edulevelsQuery = edulevelsQuery.Where(x => x.LookupId == lookupId);
            }
            var edulevels = edulevelsQuery.DistinctByField(x => x.LookupId).ToList();

            // Fetch all necessary data based on criteria
            var employeesQuery = db.Person_Employee.AsQueryable();
            var placementsQuery = db.Placements.AsQueryable();

            if (int.TryParse(branchId, out int braId))
            {
                var branchEmpIds = placementsQuery.Where(x => x.BranchId == braId).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
            }

            if (int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                var placementEmpIds = placementsQuery.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
            }

            if (status == "Active")
            {
                employeesQuery = employeesQuery.Where(x => x.IsActive);
            }
            else if (status == "InActive")
            {
                employeesQuery = employeesQuery.Where(x => !x.IsActive);
            }

            var employees = employeesQuery.ToList();
            if (!employees.Any())
            {
                return list; // Early exit if no data
            }

            // Capture scoped employee IDs
            var filteredEmpIds = employees.Select(e => e.EmployeeId).ToList();

            // Step 1: Get each employee's HIGHEST education level
            var employeesWithHighestEdu = db.Education
                .Where(edu => filteredEmpIds.Contains(edu.EmployeeId)) // Database-side scoping
                .Join(db.Lookups,
                    edu => edu.EduLevel,
                    lookup => lookup.LookupId,
                    (edu, lookup) => new { edu.EmployeeId, edu.EduLevel, lookup.Index })
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    HighestEduLevelId = g.OrderByDescending(x => x.Index).FirstOrDefault().EduLevel
                })
                .ToList();

            // Step 2: Categorize Gender sets into Male, Female, and Undefined/Unassigned
            HashSet<int> maleEmpIds = new HashSet<int>(employees
                .Where(e => e.Gender == Gender.MALE)
                .Select(e => e.EmployeeId));

            HashSet<int> femaleEmpIds = new HashSet<int>(employees
                .Where(e => e.Gender == Gender.FEMALE)
                .Select(e => e.EmployeeId));

            // Step 3: Process standard education levels (Highest to Lowest)
            foreach (var level in edulevels.OrderByDescending(x => x.Index))
            {
                var empIdsInThisLevel = employeesWithHighestEdu
                    .Where(x => x.HighestEduLevelId == level.LookupId)
                    .Select(x => x.EmployeeId)
                    .ToList();

                if (empIdsInThisLevel.Any())
                {
                    int maleCount = empIdsInThisLevel.Count(id => maleEmpIds.Contains(id));
                    int femaleCount = empIdsInThisLevel.Count(id => femaleEmpIds.Contains(id));
                    int undefinedCount = empIdsInThisLevel.Count(id => !maleEmpIds.Contains(id) && !femaleEmpIds.Contains(id));

                    var summary = new EmployeeSummary
                    {
                        Criteria = isAmharic ? "የትምህርት ደረጃ": isOromo ? "Sadarkaa Barumsaa": "Education Level",
                        SummaryName = level.Description,
                        NumberOfMaleEmployee = maleCount,
                        NumberOfFemaleEmployee = femaleCount,
                        NumberOfUnspecifiedGenderEmployee = undefinedCount,
                        TotalEmployee = maleCount + femaleCount + undefinedCount
                    };

                    list.Add(summary);
                }
            }

            // Step 4: END ROW — Add "No Education Record" summary at the absolute end
            if (string.IsNullOrEmpty(eduLevel))
            {
                var educatedEmpIds = new HashSet<int>(employeesWithHighestEdu.Select(x => x.EmployeeId));

                var uneducatedEmpIds = employees
                    .Where(e => !educatedEmpIds.Contains(e.EmployeeId))
                    .Select(e => e.EmployeeId)
                    .ToList();

                if (uneducatedEmpIds.Any())
                {
                    int maleNoEduCount = uneducatedEmpIds.Count(id => maleEmpIds.Contains(id));
                    int femaleNoEduCount = uneducatedEmpIds.Count(id => femaleEmpIds.Contains(id));
                    int undefinedNoEduCount = uneducatedEmpIds.Count(id => !maleEmpIds.Contains(id) && !femaleEmpIds.Contains(id));

                    var noEduSummary = new EmployeeSummary
                    {
                        Criteria = isAmharic ? "የትምህርት ደረጃ" : isOromo ? "Sadarkaa Barumsaa" : "Education Level",
                        SummaryName = isAmharic ? "የትምህርት ማስረጃ የሌለው": isOromo ? "Galmee Barumsaa Kan Hin Qabne": "No Education Record",
                        NumberOfMaleEmployee = maleNoEduCount,
                        NumberOfFemaleEmployee = femaleNoEduCount,
                        NumberOfUnspecifiedGenderEmployee = undefinedNoEduCount,
                        TotalEmployee = maleNoEduCount + femaleNoEduCount + undefinedNoEduCount
                    };

                    list.Add(noEduSummary); // Placed at the very end of the report
                }
            }

            return list;
        }
        public List<EmployeeSummary> GetEmployeeEduLevelSummaryExisted(string branchId, string orgStructureId, string status, string eduLevel = "")
        {
            var list = new List<EmployeeSummary>();

            // Fetch the educational levels
            var edulevelsQuery = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL);
            if (!string.IsNullOrEmpty(eduLevel) && int.TryParse(eduLevel, out int lookupId))
            {
                edulevelsQuery = edulevelsQuery.Where(x => x.LookupId == lookupId);
            }
            var edulevels = edulevelsQuery.DistinctByField(x=>x.LookupId).ToList();

            // Fetch all necessary data in a single query
            var employeesQuery = db.Person_Employee.AsQueryable();
            var placementsQuery = db.Placements.AsQueryable();

            if (int.TryParse(branchId, out int braId))
            {
                var branchEmpIds = placementsQuery.Where(x => x.BranchId == braId).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
            }

            if (int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                var placementEmpIds = placementsQuery.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
            }

            if (status == "Active")
            {
                employeesQuery = employeesQuery.Where(x => x.IsActive);
            }
            else if (status == "InActive")
            {
                employeesQuery = employeesQuery.Where(x => !x.IsActive);
            }

            var employees = employeesQuery.ToList();
            if (!employees.Any())
            {
                return list; // Early exit if no data
            }

            // Step 1: Get each employee's HIGHEST education level (join with LookUp table)
            var employeesWithHighestEdu = db.Education
                .Join(db.Lookups,
                    edu => edu.EduLevel,
                    lookup => lookup.LookupId,
                    (edu, lookup) => new { edu.EmployeeId, edu.EduLevel, lookup.Index })
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    HighestEduLevelId = g.OrderByDescending(x => x.Index).FirstOrDefault().EduLevel // Highest Index = highest education
    })
                .ToList();

            // Step 2: Pre-filter male & female employee IDs (HashSet for O(1) lookups)
            HashSet<int>maleEmpIds =  new HashSet<int>( employees
                .Where(e => e.Gender == Gender.MALE)
                .Select(e => e.EmployeeId));

            HashSet<int> femaleEmpIds = new HashSet<int>( employees
                .Where(e => e.Gender == Gender.FEMALE)
                .Select(e => e.EmployeeId));

            // Step 3: Process levels from HIGHEST to LOWEST (optional, but logical)
            foreach (var level in edulevels.OrderByDescending(x => x.Index))
            {
                // Employees whose HIGHEST education is this level
                var empIdsInThisLevel = employeesWithHighestEdu
                    .Where(x => x.HighestEduLevelId == level.LookupId)
                    .Select(x => x.EmployeeId)
                    .ToList();

                var summary = new EmployeeSummary
                {
                    Criteria = "Education Level",
                    SummaryName = level.Description,
                    NumberOfMaleEmployee = empIdsInThisLevel.Count(id => maleEmpIds.Contains(id)),
                    NumberOfFemaleEmployee = empIdsInThisLevel.Count(id => femaleEmpIds.Contains(id)),
                };

                summary.TotalEmployee = summary.NumberOfMaleEmployee + summary.NumberOfFemaleEmployee;

                if (summary.TotalEmployee > 0)
                {
                    list.Add(summary);
                }
            }
            // Step 4: Add summary for employees with NO education record
            // Skip this category if a specific education level filter was passed in the request
            if (string.IsNullOrEmpty(eduLevel))
            {
                var educatedEmpIds = new HashSet<int>(employeesWithHighestEdu.Select(x => x.EmployeeId));
                var uneducatedEmpIds = employees
                    .Where(e => !educatedEmpIds.Contains(e.EmployeeId))
                    .Select(e => e.EmployeeId)
                    .ToList();

                var noEduSummary = new EmployeeSummary
                {
                    Criteria = "Education Level",
                    SummaryName = "No Education Record",
                    NumberOfMaleEmployee = uneducatedEmpIds.Count(id => maleEmpIds.Contains(id)),
                    NumberOfFemaleEmployee = uneducatedEmpIds.Count(id => femaleEmpIds.Contains(id))
                };

                noEduSummary.TotalEmployee = noEduSummary.NumberOfMaleEmployee + noEduSummary.NumberOfFemaleEmployee;

                if (noEduSummary.TotalEmployee > 0)
                {
                    list.Add(noEduSummary);
                }
            }
            return list;
            
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    
}