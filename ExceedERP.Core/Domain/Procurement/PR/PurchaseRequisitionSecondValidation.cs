using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionSecondValidation : VoucherValidation
    {
        public int PurchaseRequisitionSecondValidationId { get; set; }
        public int PurchaseRequisitionID { get; set; }

        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
    }
}