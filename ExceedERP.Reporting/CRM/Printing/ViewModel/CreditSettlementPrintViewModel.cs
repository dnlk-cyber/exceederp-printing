using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
    public class CreditSettlementPrintViewModel
    {
        public int VoucherNumber { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public DateTime Date { get; set; }
        public string ReceiptMethod { get; set; }
        public string Remark { get; set; }

        public string PreparedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public List<CreditSettlementPrintLineItemViewModel> LineItems { get; set; } = new List<CreditSettlementPrintLineItemViewModel>();
    }

    public class CreditSettlementPrintLineItemViewModel
    {
        public int InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Witholding { get; set; }
        public decimal NetAmount { get; set; }
    }
}
