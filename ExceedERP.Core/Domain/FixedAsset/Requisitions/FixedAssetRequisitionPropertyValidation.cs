using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Requisitions
{
    public class FixedAssetRequisitionPropertyValidation : TrackUserSettingOperation
    {
        public int FixedAssetRequisitionPropertyValidationId { get; set; }
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }
}