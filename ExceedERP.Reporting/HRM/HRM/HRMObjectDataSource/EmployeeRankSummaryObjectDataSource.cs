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
    public class EmployeeRankSummaryObjectDataSource : List<EmployeeSummary>
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

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }

            if (status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
            }

            var rankList = db.Ranks.ToList();

            if (job != null)
            {
                int.TryParse(job, out int rankId);
                rankList = rankList.Where(x => x.RankId == rankId).ToList();
            }

            foreach (var r in rankList)
            {
                employee = new EmployeeSummary();
                employee.Criteria = "Rank";

                employee.SummaryName = r.RankName;

                employee.NumberOfMaleEmployee = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Where(x => x.RankId == r.RankId).OrderByDescending(x=>x.AssignmentDate).DistinctByField(x=>x.EmployeeId).Count();
                employee.NumberOfFemaleEmployee = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Where(x => x.RankId == r.RankId).OrderByDescending(x => x.AssignmentDate).DistinctByField(x => x.EmployeeId).Count();

                employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

                list.Add(employee);
            }

            return list;
        }
    }
}