using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class MonthlyLeavePlanDetail: TrackUserOperation
    {
        public int MonthlyLeavePlanDetailId { get; set; }
        public int MonthlyLeavePlanId { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}
