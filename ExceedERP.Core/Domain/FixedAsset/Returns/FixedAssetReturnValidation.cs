using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Returns
{
    public class FixedAssetReturnValidation : VoucherValidation
    {
        public int FixedAssetReturnValidationId { get; set; }
        public int FixedAssetReturnId { get; set; }
        public virtual FixedAssetReturn FixedAssetReturn { get; set; }
    }
}