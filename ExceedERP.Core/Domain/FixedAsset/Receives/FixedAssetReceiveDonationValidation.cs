using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveDonationValidation : TrackUserOperation
    {
        public int FixedAssetReceiveDonationValidationID { get; set; }
        public int FixedAssetReceiveDonationId { get; set; }
        public virtual FixedAssetReceiveDonation FixedAssetReceiveDonation { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public string Remark { get; set; }
    }
}