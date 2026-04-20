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
    public class EmployeeEduLevelSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduLevelSummary(string branchId, string orgStructureId, string status, string eduLevel = "")
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
            if (!employees.Any() || !edulevels.Any())
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

            return list;
            
        }
    }

    
}