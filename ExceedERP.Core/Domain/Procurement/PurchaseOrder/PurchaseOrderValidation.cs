using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PurchaseOrderValidation : TrackUserOperation
    {
        public int PurchaseOrderValidationID { get; set; }
        public int PurchaseOrderID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
