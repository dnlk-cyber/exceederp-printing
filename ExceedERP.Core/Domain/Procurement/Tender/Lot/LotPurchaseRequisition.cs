using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class LotPurchaseRequisition : TrackUserOperation
    {
        public int LotPurchaseRequisitionId { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        public int PurchaseRequisitionID { get; set; }
        public int StoreRequisitionID { get; set; }
        public int PurchaseRequisitionAssignmentId { get; set; }
    }
}
