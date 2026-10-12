using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeRankSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeRankSummary(string branchId, string orgStructureId, string status, string job = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
            List<Person_Employee> employees;

            var placements = db.Placements.ToList();
            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
               // placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
                placements = placements.Where(x=>x.BranchId==braId).ToList();
            }

            if (!string.IsNullOrEmpty(orgStructureId))
            {
                int.TryParse(orgStructureId, out int orgId);
               // placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                placements = placements.Where(x=>x.OrgStructureId ==orgId||(int)x.ParentOrgStructureId==orgId).ToList();
            }

            if (status == "Active")
            {
                 //var employeeIds =  db.Person_Employee.Where(x=>x.IsActive).Select(x=>x.EmployeeId)
               // placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
                placements = placements.Where(x=>x.isCurrent).ToList();
            }
            if (status == "InActive")
            {
                placements = placements.Where(x=>!x.isCurrent).ToList();
            }
            if (placements.Any())
            {

                var rankIds = placements.Select(x => x.RankId).ToList().Distinct();
                var employeeIds = placements.Select(x => x.EmployeeId).ToList().Distinct();
                var rankList = db.Ranks.Where(x => rankIds.Contains(x.RankId)).ToList();
                var maleEmployeeIds = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.MALE).Select(x=>x.EmployeeId);
                var femaleEmployeeIds = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.FEMALE).Select(x => x.EmployeeId);
                if (!string.IsNullOrEmpty(job) && rankList.Any())
                {
                    int.TryParse(job, out int rankId);
                    rankList = rankList.Where(x => x.RankId == rankId).ToList();

                }
                foreach (var r in rankList)
                {
                    employee = new EmployeeSummary();
                    employee.Criteria = "Rank";
                    employee.SummaryName = r.RankName;
                   // employee.NumberOfMaleEmployee = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.MALE).Count();
                    employee.NumberOfMaleEmployee = maleEmployeeIds.Any()? placements.Where(x =>x.RankId==r.RankId && maleEmployeeIds.Contains(x.EmployeeId)).Count():0;
                    //employee.NumberOfFemaleEmployee = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.FEMALE).Count();
                    employee.NumberOfFemaleEmployee = femaleEmployeeIds.Any()?placements.Where(x => x.RankId == r.RankId && femaleEmployeeIds.Contains(x.EmployeeId)).Count():0;
                    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

                    list.Add(employee);
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