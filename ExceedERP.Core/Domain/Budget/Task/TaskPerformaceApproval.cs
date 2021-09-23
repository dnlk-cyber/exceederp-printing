using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPerformaceApproval : TrackUserOperation
    {

        public int TaskPerformaceApprovalId { get; set; }
        public int TaskPerformaceId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

      
    }
}
