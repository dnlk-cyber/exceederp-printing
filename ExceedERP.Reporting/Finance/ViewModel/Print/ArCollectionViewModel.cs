using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ArCollectionViewModel : Organization

    {
        public string VoucherId { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal ReceiptAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ReceiptMethod { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class ArCollectionLinesViewModel
    {
        public bool IsPrepayment { get; set; }
        public string InvoiceNumber { get; set; }
        public string InstallementNo { get; set; }
        public decimal ApplyAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public DateTime ApplyDate { get; set; }
        public string TaxType { get; set; }
        public decimal TaxAmount { get; set; }
        public string WithholdingType { get; set; }
        public decimal WithholdingAmount { get; set; }
        public bool WithholdTax { get; set; }
        public decimal WithholdTaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public bool HasPrepayment { get; set; }
        public decimal PrepaymentAmount { get; set; }
    }

   
}
