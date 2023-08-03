using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PaymentRequest
{
    public class PurchasePaymentAttachment : TrackUserOperation
    {
        public int PurchasePaymentAttachmentId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}