using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveDonationDistribution :  VoucherDistribution
    {
        public int FixedAssetReceiveDonationDistributionID { get; set; }
        public int FixedAssetReceiveDonationId { get; set; }
        public virtual FixedAssetReceiveDonation FixedAssetReceiveDonation { get; set; }
        
    }
}