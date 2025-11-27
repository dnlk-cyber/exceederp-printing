using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.SelfCare;
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
        public List<EmployeeSummary> GetLeaveSummary(string leaveType,string period,string branchId,string orgStructureId,
                                                     string status,DateTime? from, DateTime? to)
        {
            
            IQueryable<Placement> placements = db.Placements;

            if (int.TryParse(branchId, out int branch))
                placements = placements.Where(x => x.BranchId == branch);

            if (int.TryParse(orgStructureId, out int org))
                placements = placements.Where(x => x.OrgStructureId == org);

            if (status == "Active")
            {
                placements = placements.Where(x => x.isCurrent);
            }
            else if (status == "InActive")
            {
                var inactiveEmployeeIds = db.Person_Employee
                                            .Where(x => !x.IsActive)
                                            .Select(x => x.EmployeeId)
                                            .ToList();

                placements = placements.Where(x => inactiveEmployeeIds.Contains(x.EmployeeId));
            }
           
            IQueryable<LeaveRequest> leaves = db.LeaveRequest.Where(x=>x.IsOnlineApproved);

            leaves = ApplyDateRangeFilter(leaves, from, to);

            if (int.TryParse(period, out int fiscalYear))
                leaves = leaves.Where(x => x.GlFiscalYearId == fiscalYear);

            if ( !string.IsNullOrEmpty(leaveType))
                leaves = leaves.Where(x => x.Type.ToString() == leaveType);
           
            var distinctLeaveEmployeeIds = leaves
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            if (!distinctLeaveEmployeeIds.Any())
                return new List<EmployeeSummary>();

           
            var placementList = placements.ToList();

           
            var maleIds = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placementList)
                                        .Select(x => x.EmployeeId)
                                        .ToList();

            var femaleIds = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placementList)
                                          .Select(x => x.EmployeeId)
                                          .ToList();

           
            var summary = new EmployeeSummary
            {
                NumberOfMaleEmployee = distinctLeaveEmployeeIds.Count(id => maleIds.Contains(id)),
                NumberOfFemaleEmployee = distinctLeaveEmployeeIds.Count(id => femaleIds.Contains(id))
            };

            summary.TotalEmployee = summary.NumberOfMaleEmployee + summary.NumberOfFemaleEmployee;

            return new List<EmployeeSummary> { summary };
        }



        /// <summary>
        /// EF 4.8-compatible From/To filtering
        /// </summary>
        public IQueryable<LeaveRequest> ApplyDateRangeFilter(
            IQueryable<LeaveRequest> query,
            DateTime? from,
            DateTime? to)
        {
            if (from != null && to == null)
                return query.Where(x => x.From >= from);

            if (from == null && to != null)
                return query.Where(x => x.To <= to);

            if (from != null && to != null)
            {
                DateTime f = from.Value;
                DateTime t = to.Value;

                return query.Where(x =>
                    (x.From >= f && x.From <= t) ||
                    (x.To >= f && x.To <= t) ||
                    (x.From <= f && x.To >= t)
                );
            }

            return query;
        }

    }
}