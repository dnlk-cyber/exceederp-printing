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

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeJobObjectDataSource : List<EmployeeJob>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeJobTitle> GetEmployeeJob(string jobTitleId, string branchId, string orgStructureId)
        {
            List<EmployeeJobTitle> EmployeeInfoList = new List<EmployeeJobTitle>();
            EmployeeJobTitle EmployeeInfo = new EmployeeJobTitle();

           // var placements = db.Placements.Where(x => x.isCurrent).ToList();//.DistinctByField(x => x.OrgStructureId).ToList();

            //var placements = db.Database
            //    .SqlQuery<Placement>
            //    (
            //        "SELECT * " +
            //        " FROM HRM.Placement " +
            //        " WHERE IsCurrent = 'True'").ToList();

            //if (branchId != null)
            //{
            //    int.TryParse(branchId, out int braId);
            //    placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            //}
            //if (orgStructureId != null)
            //{
            //    int.TryParse(orgStructureId, out int orgId);
            //    placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            //}
            //if (jobTitleId != null)
            //{
            //    int.TryParse(jobTitleId, out int lookUpId);
            //    placements = placements.Where(x => x.LookupId == lookUpId).ToList();
            //}

            var departments = db.MainCompanyStructures.ToList();

            foreach (var p in departments)
            {
                db = new ExceedDbContext();

                EmployeeInfo = new EmployeeJobTitle();

                var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == p.OrgStructureId);
                if (deptt != null)
                {
                    EmployeeInfo.Department = deptt.Name;
                }

                var oneDepPlacements = db.Placements.Where(x => x.OrgStructureId == p.OrgStructureId).ToList();

                if (branchId != null)
                {
                    int.TryParse(branchId, out int braId);
                    oneDepPlacements = oneDepPlacements.Where(x => x.BranchId == braId).ToList();
                }
                if (orgStructureId != null)
                {
                    int.TryParse(orgStructureId, out int orgId);
                    oneDepPlacements = oneDepPlacements.Where(x => x.OrgStructureId == orgId).ToList();
                }
                if (jobTitleId != null)
                {
                    int.TryParse(jobTitleId, out int lookUpId);
                    oneDepPlacements = oneDepPlacements.Where(x => x.LookupId == lookUpId).ToList();
                }

                foreach (var place in oneDepPlacements)
                {
                    var employeeJob = new EmployeeJob();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                    if (person != null)
                    {
                        employeeJob.Sex = HrHelper.GetGender(person.Gender);
                        employeeJob.FullName = HrHelper.GetEmployeeFullName(person.EmployeeId); ;
                        employeeJob.EmployeeId = person.EmployeeCode;
                    }

                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == place.RankId);
                    if (rank != null)
                    {
                        employeeJob.Rank = rank.RankName;
                    }
                    var step = db.Steps.FirstOrDefault(x => x.StepId == place.StepId);
                    if (step != null)
                    {
                        employeeJob.Step = step.StepName;
                    }
                    var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                    if (jobTitle != null)
                    {
                        employeeJob.JobTitle = jobTitle.Description;
                    }

                    employeeJob.Salary = place.Salary;
                    employeeJob.Remark = place.Remark;

                    EmployeeInfo.EmployeeJobs.Add(employeeJob);
                }

                if(oneDepPlacements.Any())
                {
                    EmployeeInfoList.Add(EmployeeInfo);
                }               
            }

            return EmployeeInfoList;
        }
    }
}