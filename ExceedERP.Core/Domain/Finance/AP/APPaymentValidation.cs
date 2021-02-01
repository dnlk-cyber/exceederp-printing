using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaymentValidation : TrackUserOperation
    {
        public int ApPaymentValidationId { get; set; }
        public int ApPaymentId { get; set; }
        public virtual ApPayment ApPayment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
