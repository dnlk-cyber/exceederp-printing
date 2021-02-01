using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractPaymentAttachment : TrackUserOperation
    {
        public int ContractPaymentAttachmentId { get; set; }
        public int ContractPaymentId { get; set; }
        public virtual ContractPayment ContractPayment { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}