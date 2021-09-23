using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{

    public class BudgetRequestTransferDistribution : VoucherDistribution
    {

        public int BudgetRequestTransferDistributionId { get; set; }
        public int BudgetRequestId { get; set; }
        public virtual BudgetRequest BudgetRequest { get; set; }
    }
 
}
