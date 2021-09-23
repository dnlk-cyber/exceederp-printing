using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetPlanApproval : TrackUserOperation
    {

        public int BudgetPlanApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

       
    }
}
