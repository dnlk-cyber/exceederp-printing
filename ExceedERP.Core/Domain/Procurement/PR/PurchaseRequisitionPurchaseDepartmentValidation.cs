using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionPurchaseDepartmentValidation : TrackUserSettingOperation
    {
        public int PurchaseRequisitionPurchaseDepartmentValidationID { get; set; }
        public int PurchaseRequisitionID { get; set; }
        public string Approver { get; set; }

        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

    }
}