using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractPaymentValidation : TrackUserOperation
    {
        public int ContractPaymentValidationId { get; set; }
        public int ContractPaymentId { get; set; }
        public virtual ContractPayment ContractPayment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}