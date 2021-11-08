using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetLeaveSummary(string leaveType, string period, string branchId, string orgStructureId, string status, DateTime? from, DateTime? to)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();

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

            #region set Leave

            LeaveType? leave = null;
            switch (leaveType)
            {
                case "ANNUAL":
                    leave = LeaveType.ANNUAL;
                    break;
                case "OTHER":
                    leave = LeaveType.OTHER;
                    break;
                case "MARRIAGE":
                    leave = LeaveType.MARRIAGE;
                    break;
                case "MATERNITY":
                    leave = LeaveType.MATERNITY;
                    break;
                case "PATERNITY":
                    leave = LeaveType.PATERNITY;
                    break;
                case "SICK_LEAVE":
                    leave = LeaveType.SICK_LEAVE;
                    break;
            }

            #endregion

            var leaveLists = GetLeaveInRange(from, to);

            if (period != null)
            {
                int.TryParse(period, out int glfisicalYearId);
                leaveLists = leaveLists.Where(x => x.GlFiscalYearId == glfisicalYearId).ToList();
            }
            if (leaveType != null)
            {
                leaveLists = leaveLists.Where(x => x.LeaveType == leave).ToList();
            }

            var malesId = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();
            var femalesId = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();

            if (leaveLists.Any())
            {
                employee.NumberOfMaleEmployee = leaveLists.Where(x => malesId.Contains(x.EmployeeId)).Count();
                employee.NumberOfFemaleEmployee = leaveLists.Where(x => femalesId.Contains(x.EmployeeId)).Count();
                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);
            }

            return list;
        }

        public List<Leave> GetLeaveInRange(DateTime? from, DateTime? to)
        {
            var leaveLists = db.Leaves.DistinctByField(x => x.EmployeeId).ToList();

            if (from != null && to == null)
            {
                leaveLists = leaveLists.Where(x => x.From >= from).ToList();
            }
            else if (from == null && to != null)
            {
                leaveLists = leaveLists.Where(x => x.To <= to).ToList();
            }
            else if (from != null && to != null)
            {
                leaveLists = leaveLists.Where(x => (x.From.Date >= from && x.From.Date <= to) || (x.To.Date >= from && x.To.Date <= to) || (x.From.Date <= from && x.To.Date >= to)).ToList();
            }

            return leaveLists;
        }
    }
}