using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderDistribution : VoucherDistribution
    {
        public int ArSalesOrderDistributionId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
    }
}
