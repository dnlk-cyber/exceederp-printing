using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractPaymentGrn : TrackUserOperation
    {
        public int ContractPaymentGrnId { get; set; }
        public int ContractPaymentId { get; set; }
        public virtual ContractPayment ContractPayment { get; set; }
        public int GrnNo { get; set; }
        public PurchaseItemType ItemType { get; set; }
        
    }
}