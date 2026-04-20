using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Localization;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    public class EmployeeJobTitleSummaryObjectDataSource : IDisposable
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeJobTitleSummary(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        {
            var list = new List<EmployeeSummary>();

            // Base employees (Person_Employee) → ensures consistency
            var employeesQuery = db.Person_Employee.AsQueryable();
            // Placements (current ones only, we’ll left join later)
            var placementsQuery = db.Placements.Where(x => x.isCurrent).AsQueryable();
            // Left join employees with placements (ensures all employees are included)
            var empWithPlacements = from emp in employeesQuery
                                    join pl in placementsQuery
                                         on emp.EmployeeId equals pl.EmployeeId into empPl
                                    from pl in empPl.DefaultIfEmpty()
                                    select new
                                    {
                                        emp.EmployeeId,
                                        emp.Gender,
                                        emp.IsActive,
                                        pl.OrgStructureId,
                                        pl.BranchId,
                                        pl.BusinessUnitId,
                                        pl.LookupId,
                                        JobLookupId = (int?)pl.LookupId
                                    };
            if (status == "Active")
            {
                var placementsEmpIds = new HashSet<int>(db.Placements.Where(x => x.isCurrent).Select(x => x.EmployeeId)).Distinct();

                empWithPlacements = empWithPlacements.Where(x => placementsEmpIds.Contains(x.EmployeeId)&& x.IsActive);

            }
            else if (status == "InActive")
            {
                var profileEmpIds = new HashSet<int>(employeesQuery.Where(x => !x.IsActive).Select(x => x.EmployeeId));


                empWithPlacements = empWithPlacements.Where(x => profileEmpIds.Contains(x.EmployeeId) && !x.IsActive);
            }
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                empWithPlacements = empWithPlacements.Where(x => x.BranchId == braId);
            }
            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
            {
                empWithPlacements = empWithPlacements.Where(x => x.BusinessUnitId == bUnitId);
            }
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures
                                     .Where(x => x.Parent == orgId)
                                     .Select(x => x.OrgStructureId)
                                     .ToList();

                empWithPlacements = empWithPlacements.Where(x => structureIds.Contains(x.OrgStructureId));
            }
            if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
            {
                empWithPlacements = empWithPlacements.Where(x => x.LookupId == lookUpId);
            }

          

            // Group by Job Title (including nulls)
            var grouped = empWithPlacements
                .GroupBy(x => x.JobLookupId)
                .Select(g => new
                {
                    JobId = g.Key,
                    Male = g.Count(e => e.Gender == Gender.MALE),
                    Female = g.Count(e => e.Gender == Gender.FEMALE)
                })
                .ToList();

            // Fetch job title lookups
            var lookupsQuery = db.Lookups.Where(x => (x.LookupType == LookUpTypes.JOB_TITLE
                                                        || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                        || x.LookupType == LookUpTypes.Jobtitle_External_Experience) && x.IsActive);
            var lookups = lookupsQuery.ToList();

            int index = 0;

            foreach (var g in grouped)
            {
                var lookup = g.JobId.HasValue
                    ? lookups.FirstOrDefault(l => l.LookupId == g.JobId.Value)
                    : null;

                list.Add(new EmployeeSummary
                {
                    index = index++,
                    SummaryName = lookup?.Description ?? Resources.Navigator_Employee_Not_Placed_Employee,
                    NumberOfMaleEmployee = g.Male,
                    NumberOfFemaleEmployee = g.Female,
                    TotalEmployee = g.Male + g.Female
                });
            }

            return list;
        }


        public void Dispose()
        {
            db.Dispose();
        }

    }
}