using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
 
    public class ApDirectPayment : Operations
    {
        public int ApDirectPaymentId { get; set; }
        [Display(Name = "Source ID")]
        public string SourceId { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "Is New Supplier")]
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Required]
        [Display(Name = "Payment Type")]
        public PaymentDocType PaymentType { get; set; }
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Purchaser")]
        public int Purchaser { get; set; }
        [Display(Name = "GRN/Work Order")]
        public string Grn { get; set; }
        [Display(Name = "PO")]
        public string Po { get; set; }
        [Display(Name = "Is Direct Payment")]
        public bool IsDirectPayment { get; set; }
        public bool IsPostedToSecondLedger { get; set; }
        public virtual ICollection<ApDirectPaymentLine> ApDirectPaymentLines { get; set; }
        public virtual ICollection<ApDirectPaymentDistribution> ApDirectPaymentDistribution { get; set; }
        public virtual ICollection<ApDirectPaymentValidation> ApDirectPaymentValidation { get; set; }
    }

    public class ApDirectPaymentLine : TrackUserSettingOperation
    {
        public int ApDirectPaymentLineId { get; set; }
        public int ApDirectPaymentId { get; set; }
        public virtual ApDirectPayment ApDirectPayment { get; set; }
        [Required]
        [Display(Name = "Purpose of Payment")]
        public string Purpose { get; set; }
        [Display(Name = "Functional Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [Display(Name = "Rate")]
        public decimal ExchangeRate { get; set; }
        [Display(Name = "Amount in Forex")]
        public decimal ForexAmount { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Required]
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Display(Name = "Withholding Type")]
        public int WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax (VAT,TOT...)?")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Net Payable")]
        public decimal NetPayable { get; set; }
        [Display(Name = "Deduction")]
        public decimal DeductAmount { get; set; }//after new
        public string Remark { get; set; }
       
    }
}
