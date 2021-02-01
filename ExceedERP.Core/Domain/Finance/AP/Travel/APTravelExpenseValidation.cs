using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelExpenseValidation : VoucherValidation
    {
        public int ApTravelExpenseValidationId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }

    }
}
