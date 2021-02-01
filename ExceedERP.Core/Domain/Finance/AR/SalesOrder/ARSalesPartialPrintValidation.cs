using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderPartialPrintValidation : VoucherValidation
    {
        public int ArSalesOrderPartialPrintValidationId { get; set; }
        public int ArSalesOrderPartialPrintId { get; set; }
        public virtual ArSalesOrderPartialPrint ArSalesOrderPartialPrint { get; set; }
    }
}
