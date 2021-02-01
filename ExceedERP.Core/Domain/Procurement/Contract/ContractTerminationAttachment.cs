using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractTerminationAttachment : TrackUserOperation
    {
        public int ContractTerminationAttachmentId { get; set; }
        public int ContractTerminationId { get; set; }
        public virtual ContractTermination ContractTermination { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}