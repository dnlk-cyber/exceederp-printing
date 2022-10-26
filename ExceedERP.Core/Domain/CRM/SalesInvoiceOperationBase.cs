using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesInvoiceOperationBase : Operations
    {
        public bool IsSendForHold { get; set; }
        public DateTime? SendForHoldTime { get; set; }
        public string SendForHoldBy { get; set; }

        public bool IsReceiptPrinted { get; set; }
        public DateTime? InvoicePrintTime { get; set; }
        public string InvoicePrintedBy { get; set; }
    }
}
