using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApPaymentSettlementGrn : TrackUserOperation
    {
        public int ApPaymentSettlementGrnId { get; set; }
        public int ApPaymentSettlementId { get; set; }
        public virtual ApPaymentSettlement ApPaymentSettlement { get; set; }
        public int GrnNo { get; set; }
        public PurchaseItemType ItemType { get; set; }
        
    }
}