using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Issues
{
    public class FixedAssetIssueValidation : TrackUserSettingOperation
    {
        public int FixedAssetIssueValidationID { get; set; }
        public int FixedAssetIssueID { get; set; }
        public virtual FixedAssetIssue FixedAssetIssue { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}