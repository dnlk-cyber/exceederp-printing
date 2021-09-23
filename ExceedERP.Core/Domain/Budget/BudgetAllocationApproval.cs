using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetAllocationApproval : TrackUserOperation
    {
        public int BudgetAllocationApprovalId { get; set; }
        public int BudgetAllocationId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public virtual BudgetAllocation BudgetAllocation { get; set; }
    }
}
