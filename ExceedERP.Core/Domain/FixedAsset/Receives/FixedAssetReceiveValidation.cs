using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveValidation : TrackUserOperation
    {
        public int FixedAssetReceiveValidationID { get; set; }
        public int FixedAssetReceiveId { get; set; }
        public virtual FixedAssetReceive FixedAssetReceive { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }
}