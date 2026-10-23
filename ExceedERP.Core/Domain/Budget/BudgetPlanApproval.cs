using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetPlanDepartmentApproval : TrackUserOperation
    {

        public int BudgetPlanDepartmentApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }


    }
    public class BudgetPlanApproval : TrackUserOperation
    {

        public int BudgetPlanApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

       
    }
    public class BudgetPlanRevisionApproval : TrackUserOperation
    {

        public int BudgetPlanRevisionApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }


    }
}
