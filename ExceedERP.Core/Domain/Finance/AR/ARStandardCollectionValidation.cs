using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArStandardCollectionValidation : TrackUserOperation
    {
        public int ArStandardCollectionValidationId { get; set; }
        public int ArStandardCollectionId { get; set; }
        public virtual ArStandardCollection ArStandardCollection { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
