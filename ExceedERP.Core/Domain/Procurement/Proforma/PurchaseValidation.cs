using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class PurchaseValidation : TrackUserOperation
    {
        public int PurchaseValidationID { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
