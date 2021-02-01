using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseValidation : TrackUserOperation
    {
        public int ForeignPurchaseValidationId { get; set; }
        public int ForeignPurchaseId { get; set; }
        public virtual Procurement.ForeignPurchase.ForeignPurchase ForeignPurchase { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}