using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractAttachment : TrackUserOperation
    {
        public int ContractAttachmentId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}