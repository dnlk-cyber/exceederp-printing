using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderValidation : VoucherValidation
    {
        public int ArSalesOrderValidationId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
    }
}
