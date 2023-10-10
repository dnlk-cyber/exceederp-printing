using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public enum PurchaseType
    {
         PurchaseOrder,
         TenderAward
    }
    public class ApPayment : Operations
    {
        public int ApPaymentId { get; set; }
        [Display(Name = "Is Prepayment?")]
        public bool IsPrepayment { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
      
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value can't be negative number.")]
        [Display(Name = "Request Amount")]
        public decimal InvoiceAmount { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value can't be negative number.")]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }
        [Display(Name = "Remaining Balance")]
        public decimal RemainingBalance { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Required]
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        //prepaid
        [Display(Name = "Has Prepayment)")]
        public bool HasPrepayment { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value can't be negative number.")]
        [Display(Name = "Prepayment Amount")]
        public decimal PrepaymentAmount { get; set; }
        [Display(Name = "Collected By Purchaser?")]
        public bool CollectedByPurchaser { get; set; }
        [Display(Name = "Purchaser")]
        public int? EmployeeId { get; set; }
        [Display(Name = "Purchase Source")]
        public PurchaseType PoTypeEnum { get; set; }
        [Display(Name = "Payment Source")]
        public string PoType { get; set; }
        [Display(Name = "Purchase Reference")]//payment req
        public string PurchaseReference { get; set; }
        [DisplayName("Tax Type")]
        public int? GLTaxRateID { get; set; }
        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }
        
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
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        [Display(Name = "For Branch or Project")]
        public int? ForBranchId { get; set; }
        [NotMapped]
        public decimal PrepaymentBalance { get; set; }
        [NotMapped]
        public decimal OutstandingBalance { get; set; }//unpaid
        [Display(Name = "Is Settled?")]
        public bool IsSettled { get; set; }//for prepayment only

        public virtual ICollection<ApPaymentDistribution> ApPaymentDistribution { get; set; }
        public virtual ICollection<ApPaymentValidation> ApPaymentValidation { get; set; }
    }
}
