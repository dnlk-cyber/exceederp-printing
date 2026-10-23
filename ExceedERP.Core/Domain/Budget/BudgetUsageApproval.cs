using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetUsageApproval : VoucherValidation
    {
         public int BudgetUsageApprovalId { get; set; }
         public int BudgetUsageID { get; set; }
         public virtual BudgetUsage BudgetUsage { get; set; }

    }
}
