using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveDistribution : VoucherDistribution
    {
        public int FixedAssetReceiveDistributionID { get; set; }
        public int FixedAssetReceiveId { get; set; }
        public virtual FixedAssetReceive FixedAssetReceive { get; set; }

    }
}