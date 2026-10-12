using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeJobObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeJobTitle> GetEmployeeJob(string jobTitleId,string branchId,object[] orgStructureId,
                                                     string businessUnit,string status,string employementType)
        {
            //  BASE QUERY 
            var placements = db.Placements.AsQueryable();

            //  STATUS FILTER
            if (!string.IsNullOrEmpty(status))
            {
                var jobTitleQuery = db.Lookups.Where(l =>
                    l.LookupType == LookUpTypes.JOB_TITLE ||
                    l.LookupType == LookUpTypes.OLD_JOB_TITLE);

                if (status == "Active")
                    jobTitleQuery = jobTitleQuery.Where(l => l.IsActive);
                else if (status == "InActive")
                    jobTitleQuery = jobTitleQuery.Where(l => !l.IsActive);

                var jobTitleIds = jobTitleQuery.Select(x => x.LookupId).ToList();

                placements = placements.Where(x =>
                    jobTitleIds.Contains(x.LookupId) &&
                    x.isCurrent == (status == "Active"));
            }

            // BRANCH FILTER 
            if (int.TryParse(branchId, out int braId))
                placements = placements.Where(x => x.BranchId == braId);

            // ORG STRUCTURE FILTER 
            if (orgStructureId != null && orgStructureId.Length > 0)
            {
                var orgIds = orgStructureId
                    .Select(o => int.TryParse(o?.ToString(), out int id) ? id : -1)
                    .Where(x => x > 0)
                    .ToList();

                placements = placements.Where(x => orgIds.Contains(x.OrgStructureId));
            }

            //JOB TITLE FILTER 
            if (int.TryParse(jobTitleId, out int lookUpId))
                placements = placements.Where(x => x.LookupId == lookUpId);

            // BUSINESS UNIT FILTER 
            if (int.TryParse(businessUnit, out int businessUnitId))
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId);

            // EMPLOYMENT TYPE FILTER
            if (!string.IsNullOrEmpty(employementType))
                placements = placements.Where(x =>
                    x.EmployementType.ToString() == employementType);

            // Load placements once
            var placementList = placements.ToList();

            if (!placementList.Any())
                return new List<EmployeeJobTitle>();

            //PRELOAD LOOKUPS (avoid N+1) 

            var employeeIds = placementList.Select(x => x.EmployeeId).Distinct().ToList();
            var rankIds = placementList.Select(x => x.RankId).Distinct().ToList();
            var stepIds = placementList.Select(x => x.StepId).Distinct().ToList();
            var jobIds = placementList.Select(x => x.LookupId).Distinct().ToList();
            var orgIdsUsed = placementList.Select(x => x.OrgStructureId).Distinct().ToList();
            var persons = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            var ranks = db.Ranks.Where(x => rankIds.Contains(x.RankId)).ToList();
            var steps = db.Steps.Where(x => stepIds.Contains(x.StepId)).ToList();
            var jobTitles = db.Lookups.Where(x => jobIds.Contains(x.LookupId)).ToList();
            var departments = db.MainCompanyStructures.Where(x => orgIdsUsed.Contains(x.OrgStructureId)).ToList();

            //BUILD RESULT 
            var result = new List<EmployeeJobTitle>();

            foreach (var orgId in orgIdsUsed)
            {
                var dep = departments.FirstOrDefault(d => d.OrgStructureId == orgId);
                if (dep == null) continue;

                var dto = new EmployeeJobTitle
                {
                    Department = dep.Name,
                    EmployeeJobs = new List<EmployeeJob>()
                };

                var empPlacements = placementList
                    .Where(p => p.OrgStructureId == orgId)
                    .GroupBy(p => p.EmployeeId)
                    .Select(g => g.First()) // distinct per employee
                    .ToList();

                foreach (var p in empPlacements)
                {
                    var person = persons.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
                    if (person == null) continue;

                    dto.EmployeeJobs.Add(new EmployeeJob
                    {
                        FullName = person.FirstNameEnglish + " " +
                                   person.MiddleNameEnglish + " " +
                                   person.LastNameEnglish,

                        EmployeeId = person.EmployeeCode,
                        Sex = person.Gender == Gender.FEMALE ? "Female" : "Male",
                        EmployeementType = p.EmployementType.ToString().ToLower(),
                        Rank = ranks.FirstOrDefault(r => r.RankId == p.RankId)?.RankName,
                        Step = steps.FirstOrDefault(s => s.StepId == p.StepId)?.StepName,
                        JobTitle = jobTitles.FirstOrDefault(j => j.LookupId == p.LookupId)?.Description,
                        Salary = p.Salary,
                        Remark = p.Remark
                    });
                }

                result.Add(dto);
            }

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAllDepartments(object[] orgStructureId)
        {
            if (orgStructureId == null || orgStructureId.Length == 0)
                return new List<string>();

            var dptIds = orgStructureId
                .Where(p => int.TryParse(p?.ToString(), out _))
                .Select(p => Convert.ToInt32(p))
                .ToList();

            if (!dptIds.Any())
                return new List<string>();

            var names = db.MainCompanyStructures
                          .Where(g => dptIds.Contains(g.OrgStructureId))
                          .Select(g => g.Name)
                          .Where(name => !string.IsNullOrEmpty(name))
                          .ToList();

            // Combine all names into one comma-separated string
            string combined = string.Join(", ", names);

            // Return a list with ONE string (this makes one row)
            return new List<string> { combined };
        }

        //public List<EmployeeJobTitle> GetEmployeeJob(string jobTitleId, string branchId, object[] orgStructureId,string businessUnit,string status,string employementType)
        //{
        //    List<EmployeeJobTitle> EmployeeInfoList = new List<EmployeeJobTitle>();

        //    var placements = db.Placements.AsQueryable();
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        if (status == "Active")
        //        {
        //            var jobTitleIds = db.Lookups
        //            .Where(l => (l.LookupType == LookUpTypes.JOB_TITLE
        //                            || l.LookupType == LookUpTypes.OLD_JOB_TITLE)
        //                        && l.IsActive)
        //            .Select(x => x.LookupId);
        //            placements = placements.Where(x => jobTitleIds.Contains(x.LookupId) && x.isCurrent);
        //        }
        //        if (status == "InActive")
        //        {
        //            var jobTitleIds = db.Lookups
        //            .Where(l => (l.LookupType == LookUpTypes.JOB_TITLE
        //                            || l.LookupType == LookUpTypes.OLD_JOB_TITLE)
        //                        && !l.IsActive)
        //            .Select(x => x.LookupId);
        //            placements = placements.Where(x => jobTitleIds.Contains(x.LookupId) && !x.isCurrent);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(branchId))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        placements =placements.Where(x=>x.BranchId==braId);
        //    }
        //    if (orgStructureId != null && orgStructureId.Length > 0)
        //    {
        //        List<int> OrgIds = orgStructureId
        //            .Select(b =>
        //            {
        //                if (int.TryParse(b?.ToString(), out int id))
        //                    return id;
        //                return -1; // Placeholder for invalid values

        //            })
        //            .Where(id => id != -1) // Remove invalid values
        //            .ToList();

        //        // Filter placements where OrgStructureId is in OrgIds
        //        placements = placements.Where(x => OrgIds.Contains(x.OrgStructureId));
        //    }

        //    if (!string.IsNullOrEmpty(jobTitleId))
        //    {
        //        int.TryParse(jobTitleId, out int lookUpId);
        //        placements = placements.Where(x => x.LookupId == lookUpId);
        //    }
        //    if (!string.IsNullOrEmpty(businessUnit))
        //    {
        //        int.TryParse(businessUnit, out int businessUnitId);
        //        placements = placements.Where(x => x.BusinessUnitId == businessUnitId);
        //    }
        //    if (!string.IsNullOrEmpty(employementType))
        //    {
        //        placements = placements.Where(x => x.EmployementType.ToString() == employementType);
        //    }
        //    var orgStructureIds = placements.ToList().Select(x=>x.OrgStructureId).Distinct().ToList();

        //    foreach (var departId in orgStructureIds)
        //    {
        //        EmployeeJobTitle employeeInfo = new EmployeeJobTitle();

        //        var deptt = db.MainCompanyStructures.Find(departId);
        //        if (deptt != null)
        //        {
        //            employeeInfo.Department = deptt.Name;
        //            var oneDepPlacements = placements.Where(x => x.OrgStructureId == departId).DistinctByField(x=>x.EmployeeId).ToList();
        //            employeeInfo.EmployeeJobs = new List<EmployeeJob>();
        //            foreach (var place in oneDepPlacements)
        //            {
        //                var employeeJob = new EmployeeJob();
        //                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
        //                if (person != null)
        //                {
        //                    employeeJob.Sex = person.Gender==Gender.FEMALE?"Female":"Male";
        //                    employeeJob.FullName = person.FirstNameEnglish +" "+person.MiddleNameEnglish+" "+person.LastNameEnglish; 
        //                    employeeJob.EmployeeId = person.EmployeeCode;
        //                }
        //                employeeJob.EmployeementType = place.EmployementType.ToString().ToLower();

        //                var rank = db.Ranks.FirstOrDefault(x => x.RankId == place.RankId);
        //                if (rank != null)
        //                {
        //                    employeeJob.Rank = rank.RankName;
        //                }
        //                var step = db.Steps.FirstOrDefault(x => x.StepId == place.StepId);
        //                if (step != null)
        //                {
        //                    employeeJob.Step = step.StepName;
        //                }
        //                var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
        //                if (jobTitle != null)
        //                {
        //                    employeeJob.JobTitle = jobTitle.Description;
        //                }
        //                employeeJob.Salary = place.Salary;
        //                employeeJob.Remark = place.Remark;
        //                employeeInfo.EmployeeJobs.Add(employeeJob);
        //            }

        //            EmployeeInfoList.Add(employeeInfo);

        //        }
        //    }

        //    return EmployeeInfoList;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}