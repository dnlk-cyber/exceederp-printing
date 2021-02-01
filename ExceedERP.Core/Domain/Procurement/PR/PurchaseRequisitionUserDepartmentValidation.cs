using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionUserDepartmentValidation : VoucherValidation
    {
        public int PurchaseRequisitionUserDepartmentValidationID { get; set; }
        public int PurchaseRequisitionID { get; set; }

        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
    }
}