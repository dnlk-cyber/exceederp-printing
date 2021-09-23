using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPlanApproval : TrackUserOperation
    {

       public int TaskPlanApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

      
    }
}
