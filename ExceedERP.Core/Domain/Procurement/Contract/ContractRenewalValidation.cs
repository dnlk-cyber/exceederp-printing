using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractRenewalValidation : TrackUserOperation
    {
        public int ContractRenewalValidationId { get; set; }
        public int ContractRenewalId { get; set; }
        public virtual ContractRenewal ContractRenewal { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}