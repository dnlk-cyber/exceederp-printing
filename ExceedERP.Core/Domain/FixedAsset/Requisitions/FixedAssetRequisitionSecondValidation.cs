using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Requisitions
{
    public class FixedAssetRequisitionSecondValidation : VoucherValidation
    {
        public int FixedAssetRequisitionSecondValidationId { get; set; }
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
    }
}