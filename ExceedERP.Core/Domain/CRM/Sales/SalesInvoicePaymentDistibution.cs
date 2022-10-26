using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class SalesInvoicePaymentDistibution : VoucherDistribution
    {

        public int SalesInvoicePaymentDistibutionId { get; set; }
        public int CashSalesInvoiceId { get; set; }
        public virtual CashSalesInvoice CashSalesInvoice { get; set; }
    }

    public class AdvancePaymentDistibution : VoucherDistribution
    {

        public int AdvancePaymentDistibutionId { get; set; }
        public int AdvancePaymentId { get; set; }
        public virtual AdvancePayment AdvancePayment { get; set; }
    }
}
