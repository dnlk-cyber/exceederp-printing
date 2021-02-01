using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class FixedAssetCountValidation : VoucherValidation
    {
        public int FixedAssetCountValidationId { get; set; }

        public int FixedAssetCountId { get; set; }
        public virtual FixedAssetCount FixedAssetCount { get; set; }
    }
}