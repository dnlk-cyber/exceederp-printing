using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Requisitions
{
    public class FixedAssetRequisitionValidation : VoucherValidation
    {
        public int FixedAssetRequisitionValidationId { get; set; }
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
    }
}