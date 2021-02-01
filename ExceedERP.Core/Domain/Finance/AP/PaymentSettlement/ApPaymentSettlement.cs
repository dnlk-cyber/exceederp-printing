using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
 
    public class ApPaymentSettlement : Operations
    {
        public int ApPaymentSettlementId { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "Is New Supplier")]
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
       
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "PO")]
        public string Po { get; set; }
        
        public virtual ICollection<ApPaymentSettlementLine> ApPaymentSettlementLines { get; set; }
        public virtual ICollection<ApPaymentSettlementDistribution> ApPaymentSettlementDistributions { get; set; }
        public virtual ICollection<ApPaymentSettlementValidation> ApPaymentSettlementValidations { get; set; }
    }

    public class ApPaymentSettlementLine : TrackUserSettingOperation
    {
        public int ApPaymentSettlementLineId { get; set; }
        public int ApPaymentSettlementId { get; set; }
        public virtual ApPaymentSettlement ApPaymentSettlement { get; set; }
        [Display(Name = "Direct Payment Id")]
        public int ApDirectPaymentId { get; set; }
        public virtual ApDirectPayment ApDirectPayment { get; set; }
        [Required]
        [Display(Name = "Purpose of Payment")]
        public string Purpose { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }//debit side
        public string Remark { get; set; }
       
    }
}
