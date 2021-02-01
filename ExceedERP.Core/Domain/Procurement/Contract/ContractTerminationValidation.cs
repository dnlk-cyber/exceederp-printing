using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractTerminationValidation : TrackUserOperation
    {
        public int ContractTerminationValidationId { get; set; }
        public int ContractTerminationId { get; set; }
        public virtual ContractTermination ContractTermination { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}