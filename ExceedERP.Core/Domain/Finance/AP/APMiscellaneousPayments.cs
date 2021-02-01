using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public enum PaymentDocType
    {
        PettyCash,
        Check
    }
    public class ApMiscellaneousPayment : Operations
    {
        public int ApMiscellaneousPaymentId { get; set; }
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
        [Display(Name = "Line Amount")]
        public decimal LineAmount { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public virtual ICollection<ApMiscellaneousPaymentLine> ApMiscellaneousPaymentLines { get; set; }
        public virtual ICollection<ApMiscellaneousPaymentDistribution> ApMiscellaneousPaymentDistribution { get; set; }
        public virtual ICollection<ApMiscellaneousPaymentValidation> ApMiscellaneousPaymentValidation { get; set; }
    }

    public class ApMiscellaneousPaymentLine : TrackUserSettingOperation
    {
        public int ApMiscellaneousPaymentLineId { get; set; }
        public int ApMiscellaneousPaymentId { get; set; }
        public virtual ApMiscellaneousPayment ApMiscellaneousPayment { get; set; }
        [Required]
        [Display(Name = "Purpose of Payment")]
        public string Purpose { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
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
        public string Remark { get; set; }
       
    }
}
