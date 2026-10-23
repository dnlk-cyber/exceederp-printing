using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionBudgetValidation : TrackUserSettingOperation
    {
        public int PurchaseRequisitionBudgetValidationId { get; set; }
        public int PurchaseRequisitionID { get; set; }

        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

    }
}