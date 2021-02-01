using ExceedERP.Core.Domain.Finance.AR;

namespace ExceedERP.Core.Domain.Common
{
    public class VoucherValidation : TrackUserOperation
    {
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
