using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractManagementValidation : TrackUserOperation
    {
        public int ContractManagementValidationId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}