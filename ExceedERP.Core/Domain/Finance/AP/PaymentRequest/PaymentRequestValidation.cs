using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PaymentRequest
{
    public class PaymentRequestValidation : VoucherValidation
    {
        public int PaymentRequestValidationId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }

    }

    public class PaymentRequestDepartmentValidation : VoucherValidation
    {
        public int PaymentRequestDepartmentValidationId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }

    }

    public class PaymentRequestBudgetValidation : VoucherValidation
    {
        public int PaymentRequestBudgetValidationId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }

    }
    public class PaymentRequestAuthorizeValidation : VoucherValidation
    {
        public int PaymentRequestAuthorizeValidationId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }

    }
}
