using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.PaymentRequest
{
    public class PurchasePaymentGrn : TrackUserOperation
    {
        public int PurchasePaymentGrnId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }
        public int GrnNo { get; set; }
        public PurchaseItemType ItemType { get; set; }
        
    }
}