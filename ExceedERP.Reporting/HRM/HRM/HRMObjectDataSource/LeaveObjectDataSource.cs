using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetLeaveSummary(string leaveType, object[] period,
                                                     string branchId, string orgStructureId,
                                                     string status, DateTime? from, DateTime? to)
        {
            // Base placements query
            var placements = db.Placements.AsQueryable();
            // Leave requests base query
            var leaveRequests = db.LeaveRequest.Where(x => x.IsOnlineApproved);

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                placements = placements.Where(x => x.BranchId == braId);
                leaveRequests = leaveRequests.Where(x => x.BranchId == braId);

            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = placements.Where(x => x.OrgStructureId == orgId);

            }

            if (status == "Active")
            {
                placements = placements
                    .Where(x => x.isCurrent && db.Person_Employee
                        .Any(e => e.EmployeeId == x.EmployeeId && e.IsActive));
            }
            else if (status == "InActive")
            {
                placements = placements
                    .Where(x => db.Person_Employee
                        .Any(e => e.EmployeeId == x.EmployeeId && !e.IsActive));
            }

         

            // Filter by date range (overlap logic)
            if (from.HasValue && to.HasValue)
                leaveRequests = leaveRequests.Where(x => x.From <= to.Value && x.To >= from.Value);
            else if (from.HasValue)
                leaveRequests = leaveRequests.Where(x => x.To >= from.Value);
            else if (to.HasValue)
                leaveRequests = leaveRequests.Where(x => x.From <= to.Value);

            // Filter by fiscal year (period)
            if (period != null && period.Length > 0)
            {
                var periodIds = period.Select(p => Convert.ToInt32(p)).ToList();
                leaveRequests = leaveRequests.Where(x => periodIds.Contains(x.GlFiscalYearId));
            }

            // Filter by leave type
            if (!string.IsNullOrEmpty(leaveType) && Enum.TryParse(leaveType, true, out LeaveType parsedLeave))
                leaveRequests = leaveRequests.Where(x => x.Type == parsedLeave);

            // Get employee ids from placements
            var employeeIds = placements.Select(x => x.EmployeeId).Distinct();

            // Split employees by gender (replacing EmployeeHelper)
            var maleIds = db.Person_Employee
                .Where(e => employeeIds.Contains(e.EmployeeId) && e.Gender == Gender.MALE)
                .Select(e => e.EmployeeId);

            var femaleIds = employeeIds.Except(maleIds);

            // Calculate summary
            var summary = new EmployeeSummary
            {
                NumberOfMaleEmployee = leaveRequests.Count(x => maleIds.Contains(x.EmployeeId)),
                NumberOfFemaleEmployee = leaveRequests.Count(x => femaleIds.Contains(x.EmployeeId)),
            };

            summary.TotalEmployee = summary.NumberOfMaleEmployee + summary.NumberOfFemaleEmployee;

            return summary.TotalEmployee > 0 ? new List<EmployeeSummary> { summary } : new List<EmployeeSummary>();
        }

        //public List<EmployeeSummary> GetLeaveSummary(string leaveType, object[] period, string branchId, string orgStructureId, string status, DateTime? from, DateTime? to)
        //{
        //    var placements = db.Placements.AsQueryable();

        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //        placements = placements.Where(x=>x.BranchId==braId);

        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //        placements = placements.Where(x=>x.OrgStructureId== orgId);

        //    if (status == "Active")
        //    {
        //        var empIds = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
        //        placements = placements.Where(x => empIds.Contains(x.EmployeeId)&&x.isCurrent);
        //    }

        //    //placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);

        //    else if (status == "InActive")
        //    {
        //        var empIds = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
        //        placements = placements.Where(x => empIds.Contains(x.EmployeeId));

        //    }
        //    //placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);

        //    //var leaveLists = GetLeaveInRange(from, to);
        //    var leaveRequests = db.LeaveRequest.Where(x => x.IsOnlineApproved);

        //    if (from.HasValue && to.HasValue)
        //    {
        //        // Leaves that overlap with the given range
        //        leaveRequests = leaveRequests.Where(x => x.From <= to.Value && x.To >= from.Value);
        //    }
        //    else if (from.HasValue)
        //    {
        //        leaveRequests = leaveRequests.Where(x => x.To >= from.Value);
        //    }
        //    else if (to.HasValue)
        //    {
        //        leaveRequests = leaveRequests.Where(x => x.From <= to.Value);
        //    }

        //    var leaveLists = leaveRequests.ToList();


        //    if (period != null && period.Length > 0)
        //    {
        //        var periodIds = period.Select(p => Convert.ToInt32(p)).ToList();
        //        leaveLists = leaveLists.Where(x => periodIds.Contains(x.GlFiscalYearId)).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(leaveType) && Enum.TryParse(leaveType, true, out LeaveType parsedLeave))
        //    {
        //        leaveLists = leaveLists.Where(x => x.Type == parsedLeave).ToList();
        //    }

        //    var employeeIds = new HashSet<int>( placements.Select(x => x.EmployeeId));
        //    var maleIds = new HashSet<int>( EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements.ToList()).Select(x => x.EmployeeId));
        //    var femaleIds = employeeIds.Except(maleIds);

        //    var summary = new EmployeeSummary
        //    {
        //        NumberOfMaleEmployee = leaveLists.Count(x => maleIds.Contains(x.EmployeeId)),
        //        NumberOfFemaleEmployee = leaveLists.Count(x => femaleIds.Contains(x.EmployeeId)),
        //    };
        //    summary.TotalEmployee = summary.NumberOfMaleEmployee + summary.NumberOfFemaleEmployee;

        //    return summary.TotalEmployee > 0 ? new List<EmployeeSummary> { summary } : new List<EmployeeSummary>();
        //}
        public List<LeaveRequest> GetLeaveInRange(DateTime? from, DateTime? to)
        {
            // Start with the base query
            var leaveLists = db.LeaveRequest.Where(x=>x.IsOnlineApproved).AsQueryable();

            // Apply distinct by EmployeeId
            leaveLists = leaveLists.GroupBy(x => x.EmployeeId).Select(g => g.FirstOrDefault());

            // Apply date range filters
            if (from.HasValue || to.HasValue)
            {
                leaveLists = leaveLists.Where(x =>
                    (!from.HasValue || x.From >= from.Value) &&
                    (!to.HasValue || x.To <= to.Value));
            }

            // Materialize the results as a list
            return leaveLists.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}