using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public enum InvoiceSource
    {
        Invoice,
        SalesOrder
    }
    public class ArStandardCollectionLines : TrackUserOperation
    {
        public int ArStandardCollectionLinesId { get; set; }
        public int ArStandardCollectionId { get; set; }
        public virtual ArStandardCollection ArStandardCollection { get; set; }
        [Display(Name = "Is Prepayment (Check if yes)")]
        public bool IsPrepayment { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Installment")]
        public string InstallmentNo { get; set; }
        [Display(Name = "Apply Amount")]
        public decimal ApplyAmount { get; set; }
        [Display(Name = "Balance Due")]
        public decimal BalanceDue { get; set; }
        [Display(Name = "Apply Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ApplyDate { get; set; }
        //taxes
        [Required]
        [Display(Name = "Tax Type")]
        public string TaxType { get; set; }
        [Required]
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        //
        [Display(Name = "Net Amount")]
        public decimal NetAmount { get; set; }
        //prepaid
        [Display(Name = "Has Prepayment)")]
        public bool HasPrepayment { get; set; }
        [Display(Name = "Prepayment Amount")]
        public decimal PrepaymentAmount { get; set; }

        public InvoiceSource Source { get; set; }
        public int BranchId { get; set; }
    }
}
