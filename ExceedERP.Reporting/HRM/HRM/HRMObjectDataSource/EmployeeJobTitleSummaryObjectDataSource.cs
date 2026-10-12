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

        //public List<EmployeeSummary> GetEmployeeJobTitleSummary(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        //{
        //    var list = new List<EmployeeSummary>();

        //    // Filter Lookups for Job Titles
        //    var lookupsQuery = db.Lookups
        //                         .Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive)
        //                         .AsQueryable();

        //    // Filter Placements by Criteria
        //    var empPlacementsQuery = db.Placements.AsQueryable();

        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BranchId == braId);
        //    }
        //    if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.BusinessUnitId == bUnitId);
        //    }
        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures
        //                             .Where(x => x.Parent == orgId)
        //                             .Select(x => x.OrgStructureId)
        //                             .ToList();

        //        empPlacementsQuery = empPlacementsQuery.Where(x => structureIds.Contains(x.OrgStructureId));
        //    }
        //    if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
        //    {
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.LookupId == lookUpId);
        //        lookupsQuery = lookupsQuery.Where(x => x.LookupId == lookUpId);
        //    }

        //    // Filter Employees by Status
        //    var employeesQuery = db.Person_Employee.AsQueryable();

        //    if (status == "Active")
        //    {
        //        employeesQuery = employeesQuery.Where(x => x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => x.isCurrent);
        //    }
        //    else if (status == "InActive")
        //    {
        //        employeesQuery = employeesQuery.Where(x => !x.IsActive);
        //        empPlacementsQuery = empPlacementsQuery.Where(x => !x.isCurrent);
        //    }

        //    // Get Employee IDs by Gender
        //    var maleEmpIds = employeesQuery.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
        //    var femaleEmpIds = employeesQuery.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

        //    // Get Placements and Lookups
        //    var empPlacements = empPlacementsQuery.DistinctByField(x => x.EmployeeId).ToList();
        //    var lookupIds = new HashSet<int>(empPlacements.Select(x => x.LookupId));
        //    var lookups = lookupsQuery.Where(x=>lookupIds.Contains(x.LookupId)).ToList();

        //    int index = 0;

        //    // Summarize Employees by Job Title
        //    foreach (var lookup in lookups)
        //    {
        //        int maleCount = empPlacements.Count(x => maleEmpIds.Contains(x.EmployeeId) && x.LookupId == lookup.LookupId);
        //        int femaleCount = empPlacements.Count(x => femaleEmpIds.Contains(x.EmployeeId) && x.LookupId == lookup.LookupId);

        //        if (maleCount > 0 || femaleCount > 0) // Exclude job titles with no employees
        //        {
        //            list.Add(new EmployeeSummary
        //            {
        //                index = index++,
        //                SummaryName = lookup.Description,
        //                NumberOfMaleEmployee = maleCount,
        //                NumberOfFemaleEmployee = femaleCount,
        //                TotalEmployee = maleCount + femaleCount
        //            });
        //        }
        //    }

        //    // Employees Not Placed
        //    var notPlacedMales = maleEmpIds.Count(id => !empPlacements.Any(x => x.EmployeeId == id));
        //    var notPlacedFemales = femaleEmpIds.Count(id => !empPlacements.Any(x => x.EmployeeId == id));

        //    if (notPlacedMales > 0 || notPlacedFemales > 0)
        //    {
        //        list.Add(new EmployeeSummary
        //        {
        //            index = index,
        //            SummaryName = Resources.Navigator_Employee_Not_Placed_Employee,
        //            NumberOfMaleEmployee = notPlacedMales,
        //            NumberOfFemaleEmployee = notPlacedFemales,
        //            TotalEmployee = notPlacedMales + notPlacedFemales
        //        });
        //    }

        //    return list;
        //}

        //public List<EmployeeSummary> GetEmployeeJobTitleSummary(string branchId,string bussinessUnitId, string orgStructureId, string job, string status)
        //{

        //    List<EmployeeSummary> list = new List<EmployeeSummary>();

        //    var employee = new EmployeeSummary();

        //    var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE&&x.IsActive).ToList();


        //    int index = 0;

        //    var empPlacement = db.Placements.ToList();
        //    if (branchId != null)
        //    {
        //        int.TryParse(branchId, out int braId);
        //        empPlacement = empPlacement.Where(x => x.BranchId == braId).ToList();
        //    }
        //    if (bussinessUnitId != null)
        //    {
        //        int.TryParse(bussinessUnitId, out int bUnitId);
        //        empPlacement = empPlacement.Where(x => x.BusinessUnitId == bUnitId).ToList();
        //    }
        //    if (orgStructureId != null)
        //    {
        //        int.TryParse(orgStructureId, out int orgId);
        //        // empPlacement = empPlacement.Where(x => x.OrgStructureId == orgId).ToList();
        //        empPlacement = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(empPlacement, orgId);
        //        /// and here filter jobtitles under this department

        //        var lookupIds = empPlacement.Select(x => x.LookupId).ToList();
        //        lookups = lookups.Where(x => lookupIds.Contains(x.LookupId)).ToList();

        //    }
        //    if (job != null)
        //    {
        //        int.TryParse(job, out int lookUpId);
        //        empPlacement = empPlacement.Where(x => x.LookupId == lookUpId).ToList();

        //        /// filter by jobtitle here 
        //        lookups = lookups.Where(x => x.LookupId == lookUpId).ToList();
        //    }

        //    var males = db.Person_Employee.Where(x => x.Gender == Gender.MALE).ToList();
        //    var females = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).ToList();

        //    var malesId = males.Select(x => x.EmployeeId).ToList();
        //    var femalesId = females.Select(x => x.EmployeeId).ToList();


        //    if (status == "Active")
        //    {
        //        malesId = males.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
        //        femalesId = females.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
        //        empPlacement = empPlacement.Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
        //    }
        //    if (status == "InActive")
        //    {
        //        malesId = males.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
        //        femalesId = females.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
        //        empPlacement = empPlacement.Where(x => !x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
        //    }


        //    empPlacement = empPlacement.DistinctByField(x => x.EmployeeId).ToList();

        //    foreach (var look in lookups)
        //    {
        //        employee = new EmployeeSummary();

        //        employee.index = index;
        //        employee.SummaryName = look.Description;
        //        employee.NumberOfFemaleEmployee = empPlacement.Where(x => femalesId.Contains(x.EmployeeId) && x.LookupId == look.LookupId).Count();
        //        employee.NumberOfMaleEmployee = empPlacement.Where(x => malesId.Contains(x.EmployeeId) && x.LookupId == look.LookupId).Count();
        //        employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

        //        list.Add(employee);
        //        index++;
        //    }


        //    employee = new EmployeeSummary();

        //    var countM = 0;
        //    var countF = 0;

        //    foreach (var id in malesId)
        //    {
        //        var place = db.Placements.Where(x => x.EmployeeId == id);
        //        if (place == null)
        //        {
        //            countM = countM + 1;
        //        }
        //    }

        //    foreach (var id in femalesId)
        //    {
        //        var place = db.Placements.FirstOrDefault(x => x.EmployeeId == id);
        //        if (place == null)
        //        {
        //            countF = countF + 1;
        //        }
        //    }

        //    employee.SummaryName = Resources.Navigator_Employee_Not_Placed_Employee ;
        //    employee.NumberOfMaleEmployee = countM;
        //    employee.NumberOfFemaleEmployee = countF;
        //    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

        //    employee.index = index++;

        //    list.Add(employee);

        //    return list;
        //}

        public void Dispose()
        {
            db.Dispose();
        }

    }
}