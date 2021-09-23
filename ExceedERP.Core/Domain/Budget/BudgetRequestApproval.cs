using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetRequestApproval : TrackUserOperation
    {

        public int BudgetRequestApprovalId { get; set; }
        public int BudgetRequestId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public bool IsTransfer { get; set; }
        public virtual BudgetRequest BudgetRequest { get; set; }
    }
}
