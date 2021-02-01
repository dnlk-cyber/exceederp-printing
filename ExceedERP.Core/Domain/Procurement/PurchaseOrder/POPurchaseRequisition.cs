using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Proforma;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PoPurchaseRequisition : TrackUserOperation
    {
        public int PoPurchaseRequisitionId { get; set; }
        public int PurchaseOrderID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public int PurchaseRequisitionID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int StoreRequisitionID { get; set; }
        public int PurchaseRequisitionAssignmentId { get; set; }
    }
}