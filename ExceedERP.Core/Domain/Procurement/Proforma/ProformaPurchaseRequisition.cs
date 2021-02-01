using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class ProformaPurchaseRequisition : TrackUserOperation
    {
        public int ProformaPurchaseRequisitionId { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        public int PurchaseRequisitionID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int StoreRequisitionID { get; set; }
        [Display(Name = "Purchase Requisition")]
        public int PurchaseRequisitionAssignmentId { get; set; }
    }
}