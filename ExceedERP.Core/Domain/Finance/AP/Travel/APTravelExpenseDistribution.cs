using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelExpenseDistribution : VoucherDistribution
    {
        public int ApTravelExpenseDistributionId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
    }
}
