using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.PaymentRequest
{
    public class PurchasePayment : NewOperations
    {
        public int PurchasePaymentId { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Supplier Invoice")]
        public string SupplierInvoice { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }
        [Display(Name = "Apply Penality")]
        public bool ApplyPenality { get; set; }
        [Display(Name = "Penality Amount")]
        public decimal PenalityAmount { get; set; }
        [Display(Name = "Service Charge %")]
        public decimal ServiceCharge { get; set; }
        [Display(Name = "Other Payment")]
        public decimal OtherPayment { get; set; } //stamp
        [Display(Name = "Net Payable")]
        public decimal NetPayable { get; set; }
        [Display(Name = "Remaining")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Is Paid?")]
        public bool Paid { get; set; }
        [Display(Name = "With Out GRN/SRN?")]
        public bool WithOutGRN { get; set; }        
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }        
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "For Branch or Project")]
        public int? ForBranchId { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        public string NextApprover { get; set; }
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }
        [Display(Name = "Source Type")]
        public string Type { get; set; }
        [Display(Name = "Source")]
        public string PoOrContractNumber { get; set; }        
        public string TenderRef { get; set; }
        public string ProformaRef { get; set; }
        [Display(Name = "Purchaser")]
        public int ? ProcurementOfficer { get; set; }
        [Display(Name = "Analyzer")]
        public int ? ProcurementAnalyzer { get; set; }       
        [Display(Name = "Purpose Of Payment")]
        public string PurposeOfPayment { get; set; }
        //budget controll
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        //finance controll
        public bool IsFinanceSendForApproval { get; set; }
        public string FinanceSendForApprovalBy { get; set; }
        public DateTime? FinanceSendForApprovalTime { get; set; }
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        [DisplayName("Purchase Type")]
        public PurchasePaymentType PurchasePaymentType { get; set; }
        public virtual ICollection<PurchasePaymentAttachment> PurchasetPaymentAttachements { get; set; }
        public virtual ICollection<PurchasePaymentGrn> PurchasetPaymentGrns { get; set; }
        public virtual ICollection<PurchasePaymentValidation> PurchasetPaymentValidations { get; set; }
        public virtual ICollection<PurchasePaymentBudgetValidation> PurchasePaymentBudgetValidations { get; set; }
        public virtual ICollection<PurchasePaymentFinanceValidation> PurchasePaymentFinanceValidations { get; set; }
    }

    public class PurchasePaymentBudgetValidation: VoucherValidation
    {
        public int PurchasePaymentBudgetValidationId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }       
    }

    public class PurchasePaymentFinanceValidation : VoucherValidation
    {
        public int PurchasePaymentFinanceValidationId { get; set; }
        public int PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }
    }
}