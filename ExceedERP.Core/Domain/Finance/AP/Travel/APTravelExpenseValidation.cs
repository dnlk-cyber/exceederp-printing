using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelExpenseValidation : VoucherValidation
    {
        public int ApTravelExpenseValidationId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }

    }

    public class ApTravelExpenseDepartmentValidation : VoucherValidation
    {
        public int ApTravelExpenseDepartmentValidationId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }

    }

    public class ApTravelExpenseBudgetValidation : VoucherValidation
    {
        public int ApTravelExpenseBudgetValidationId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }

    }
}
