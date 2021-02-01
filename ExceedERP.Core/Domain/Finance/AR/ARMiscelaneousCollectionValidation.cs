using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARMiscelaneousCollectionValidation : TrackUserOperation
    {
        public int ARMiscelaneousCollectionValidationID { get; set; }
        public int ARMiscelaneousCollectionID { get; set; }
        public virtual ARMiscelaneousCollection ARMiscelaneousCollection { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
