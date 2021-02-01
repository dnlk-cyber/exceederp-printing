using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
    public class AssetTransferValidation : VoucherValidation
    {
        public int AssetTransferValidationId { get; set; }
        public int AssetTransferId { get; set; }
        public virtual AssetTransfer AssetTransfer { get; set; }
    }
}