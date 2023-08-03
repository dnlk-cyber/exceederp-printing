using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApDirectPaymentValidation : TrackUserOperation
    {
        public int ApDirectPaymentValidationId { get; set; }
        public int ApDirectPaymentId { get; set; }
        public virtual ApDirectPayment ApDirectPayment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
