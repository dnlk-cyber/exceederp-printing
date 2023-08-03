using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApMiscellaneousPaymentValidation : TrackUserOperation
    {
        public int ApMiscellaneousPaymentValidationId { get; set; }
        public int ApMiscellaneousPaymentId { get; set; }
        public virtual ApMiscellaneousPayment ApMiscellaneousPayment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
