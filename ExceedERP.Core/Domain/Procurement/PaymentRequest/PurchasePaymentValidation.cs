using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PaymentRequest
{
    public class PurchasePaymentValidation : VoucherValidation
    {
        public int PurchasePaymentValidationId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }
      
    }

    public class PurchasePaymentAuthorizeValidation : VoucherValidation
    {
        public int PurchasePaymentAuthorizeValidationId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }

    }
}