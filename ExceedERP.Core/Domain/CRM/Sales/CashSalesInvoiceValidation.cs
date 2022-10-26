using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class CashSalesInvoiceValidation : VoucherValidation
    {
        [Key]
        public int CashSalesInvoiceValidationId { get; set; }

        public int CashSalesInvoiceId { get; set; }

        public virtual CashSalesInvoice CashSalesInvoice { get; set; }
    }
}
