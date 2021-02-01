using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Reporting.SharedDataSources;
using System;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ArInvoiceViewModel : Organization
    {
        public string VoucherId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public TransactionClass TransactionClass { get; set; }
        public DateTime Date { get; set; }
        public string PaymentTerm { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class ArInvoiceLinesViewModel
    {
        public string Items { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string TaxType { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
