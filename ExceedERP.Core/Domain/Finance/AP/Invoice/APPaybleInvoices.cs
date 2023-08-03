using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public enum ApInvoiceSource
    {
        PurchaseOrder,
        Contract
    }
    public enum GrnSource
    {
        ManualInvoice,
        StockGrn,
        ServiceGrn,
        FixedssetGrn
    }
    public class ApPaybleInvoices : Operations
    {
        public int ApPaybleInvoicesId { get; set; }
        [Display(Name = "Source")]
        public string SourceId { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }

        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "Purchaser")]
        public int? EmployeeId { get; set; }
        [Display(Name = "Transaction Type")]
        public ApInvoiceTransactionType Type { get; set; }
        [Display(Name = "Purchase Req.No")]
        public string PrNo { get; set; }
        [Display(Name = "PO.No/WO.No")]
        public string PoNum { get; set; }
        [Display(Name = "GR.No")]
        public string Grn { get; set; }
        [Display(Name = "Purpose of Payment")]
        public String PurposeOfPayment { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Payment Terms")]
        public int PaymentTerms { get; set; }

        // for display only
        [Display(Name = "Invoice Amount")]
        public decimal InvoiceAmount { get; set; }
        [Display(Name = "Advance Amount")]
        public decimal AdvanceAmount { get; set; }               
        [DisplayName("Tax Type")]
        public int? GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Tax Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Sub Total")]
        public decimal Subtotal { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        [Display(Name = "Net Payable Amount")]
        public decimal NetPayable { get; set; }
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Invoice Source")]
        public ApInvoiceSource InvoiceSource { get; set; }
        [Display(Name = "Invoice Status")]
        public InvoiceStatus InvoiceStatus { get; set; }
        public bool IsManual { get; set; }
        public GrnSource Source { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [Display(Name = "For Branch or Project")]
        public int? ForBranchId { get; set; }
        public virtual ICollection<ApPaybleInvoicesLines> APPaybleInvoicesLines { get; set; }
        public virtual ICollection<ApPaybleInvoiceDistribution> APPaybleInvoiceDistribution { get; set; }
        public virtual ICollection<ApPaybleInvoiceValidation> APPaybleInvoiceValidation { get; set; }
        public virtual ICollection<APPaybleInvoicePrepayment> APPaybleInvoicePrepayment { get; set; }
        public virtual ICollection<APPaymentsViewModel> APPaymentsViewModel { get; set; }
    }
}
