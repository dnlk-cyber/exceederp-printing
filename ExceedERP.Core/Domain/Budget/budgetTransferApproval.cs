using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public  class BudgetTransferApproval : TrackUserOperation
    {
        public int BudgetTransferApprovalId { get; set; }
        public int BudgetTransferId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public virtual BudgetTransfer BudgetTransfer { get; set; }
    }
}
