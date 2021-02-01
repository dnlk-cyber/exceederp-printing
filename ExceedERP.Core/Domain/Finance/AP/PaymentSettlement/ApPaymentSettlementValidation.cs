using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaymentSettlementValidation : TrackUserOperation
    {
        public int ApPaymentSettlementValidationId { get; set; }
        public int ApPaymentSettlementId { get; set; }
        public virtual ApPaymentSettlement ApPaymentSettlement { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
