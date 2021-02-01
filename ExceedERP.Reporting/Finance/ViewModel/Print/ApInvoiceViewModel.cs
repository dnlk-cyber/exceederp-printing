using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ApInvoiceViewModel : Organization

    {
        public string VoucherId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public ApInvoiceTransactionType Type { get; set; }
        //PO.No/WO.No"
        public String PoNum { get; set; }
        public String Purchaser { get; set; }
        public String GrnSrn { get; set; }
        public String PrNo { get; set; }
        public String PurposeOfPayment { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int PaymentTerms { get; set; }

        // for display only
        public decimal InvoiceAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal WithholdTaxAmount { get; set; }
        public decimal NetPayable { get; set; }
        public decimal AmountDue { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class ApInvoiceLinesViewModel 
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal NetPayable { get; set; }
        public string ReceiptNo { get; set; }
        public string TaxType { get; set; }
        public decimal TaxAmount { get; set; }
        public string WithholdingType { get; set; }
        public decimal WithholdingAmount { get; set; }
        public bool WithholdTax { get; set; }
        public decimal WithholdTaxAmount { get; set; }
        //PO.No/WO.No"
        public string PoNo { get; set; }
    }
}
