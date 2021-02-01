using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseCostValidation : TrackUserOperation
    {
        public int ForeignPurchaseCostValidationId { get; set; }
        public int ForeignPurchaseCostId { get; set; }
        public virtual ForeignPurchaseCost ForeignPurchaseCost { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}