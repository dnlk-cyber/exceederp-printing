using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public   class AdvancePaymentLog : TrackUserOperation
    {
            public int AdvancePaymentLogId { get; set; }

            public int AdvancePaymentLineId { get; set; }
            public int CashSalesInvoiceId { get; set; }
            public int LineItemId { get; set; }
            public decimal Amount { get; set; }
            public AdvancePaymentLine AdvancePaymentLine { get; set; }
    
    }
}
