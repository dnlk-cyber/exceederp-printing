using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionReassignmentValidation : VoucherValidation
    {
        public int PurchaseRequisitionReassignmentValidationId { get; set; }
        public int PurchaseRequisitionAssignmentId { get; set; }

        public virtual PurchaseRequisitionAssignment PurchaseRequisitionAssignment { get; set; }
    }
}