using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public enum SettlemetSource
    {
        NA,
        Prepayment,
        Payment,
        DirectPayment,
    }
 
    public class ApPaymentSettlement : DepartmentOperations
    {
        public int ApPaymentSettlementId { get; set; }
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }      
        [Display(Name = "By Purchaser?")]
        public bool CollectedByPurchaser { get; set; }
        [Display(Name = "Purchaser")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Reference")]
        public string Reference { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }

        public virtual ICollection<ApPaymentSettlementLine> ApPaymentSettlementLines { get; set; }
        public virtual ICollection<ApPaymentSettlementGrn> ApPaymentSettlementGrns { get; set; }
        public virtual ICollection<ApPaymentSettlementDistribution> ApPaymentSettlementDistributions { get; set; }
        public virtual ICollection<ApPaymentSettlementDepartmentValidation> ApPaymentSettlementDepartmentValidations { get; set; }
        public virtual ICollection<ApPaymentSettlementValidation> ApPaymentSettlementValidations { get; set; }
    }

    public class ApPaymentSettlementLine : TrackUserSettingOperation
    {
        public int ApPaymentSettlementLineId { get; set; }
        public int ApPaymentSettlementId { get; set; }
        public virtual ApPaymentSettlement ApPaymentSettlement { get; set; }
        public string Source { get; set; }
        
        [Display(Name = "Source Ref.")]
        public int ? SourceId { get; set; }
        public virtual ApDirectPayment ApDirectPayment { get; set; }
        [Required]
        [Display(Name = "Purpose of Payment")]
        public string Purpose { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }//debit side

        [DisplayName("Tax Type")]
        public int? GLTaxRateID { get; set; }
        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Withholding Type")]
        public int? WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        //
        [Display(Name = "Net Amount")]
        public decimal NetAmount { get; set; }      

        public string Remark { get; set; }
       
    }
}
