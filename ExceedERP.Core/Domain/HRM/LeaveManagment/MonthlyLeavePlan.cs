using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class MonthlyLeavePlan : TrackUserOperation
    {
        public int MonthlyLeavePlanId { get; set; }
        public int EmployeeId { get; set; }
        public int GlFiscalYearId { get; set; }
        
    }
}
