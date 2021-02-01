using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
    public class AssetTransferReceiveValidation : VoucherValidation
    {
        public int AssetTransferReceiveValidationId { get; set; }
        public int AssetTransferReceiveId { get; set; }
        public virtual AssetTransferReceive TransferReceive { get; set; }
    }
}