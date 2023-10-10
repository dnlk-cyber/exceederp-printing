using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PurchaseOrderDistribution : VoucherDistribution
    {
        public int PurchaseOrderDistributionID { get; set; }
        public int PurchaseOrderID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
