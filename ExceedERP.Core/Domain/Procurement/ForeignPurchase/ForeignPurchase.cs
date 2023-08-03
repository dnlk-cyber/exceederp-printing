using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{

 
    public enum ForeignPaymentMethod
    {
        LC,
        TT,
        CAD
    }
    public enum FOBMethod
    {
        NationalBank,
        MinistryOfDefense
    }

    public class ForeignPurchase : Operations
    {
        public int ForeignPurchaseId { get; set; }
        [DisplayName("Tender Reference")]
        public string TenderRef { get; set; }
        [DisplayName("LC/TT/CAD Reference")]
        public string LcTtCadRef { get; set; }
        [DisplayName("Advance")]
        public decimal Advance { get; set; }
        [DisplayName("LC")]
        public decimal LC { get; set; }
        [DisplayName("Retention")]
        public decimal Retention { get; set; }
        [DisplayName("Purchase Method Reference")]
        public int PurchaseRef { get; set; }//poid,proformaid,award id
        [DisplayName("Supplier")]
        public string Supplier { get; set; }
        [DisplayName("Item Description")]
        public string ItemDesc { get; set; }
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Method")]
        public string ProcurementMethod { get; set; }
        [Display(Name = "FOB Method")]
        public FOBMethod FOBMethod { get; set; }
        [Display(Name = "Approval Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovalDate { get; set; }
        [Display(Name = "Opening Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpenigDate { get; set; }
        [Display(Name = "Technical Document Sent Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TechnicalDocSendDate { get; set; }
        [Display(Name = "Financial Opening Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FinancialOpenigDate { get; set; }
        [Display(Name = "Minute Approved Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MinuteApprovedDate { get; set; }
        [Display(Name = "Award Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AwardDate { get; set; }
        [Display(Name = "Contract Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ContractDate { get; set; }
        [Display(Name = "Payment Method")]
        public ForeignPaymentMethod PaymentMethod { get; set; }
        [Display(Name = "Payment Method")]
        public int ForeignCurrencyProcessId { get; set; }
        [Display(Name = "Payment Open Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentOpenDate { get; set; }
        [Display(Name = "Payment Extension Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentExtensionDate { get; set; }

        [Display(Name = "Doc. Receive From Bank")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DocReceivedFromBankDate { get; set; }
        [Display(Name = "Clearance start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ClearanceStartDate { get; set; }
        [Display(Name = "Transit Process Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TransitProcessDate { get; set; }
        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; }
        [DisplayName("Delivery No.")]
        public string DeliveryNo { get; set; }
        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        public virtual ICollection<ForeignPurchaseCost> ForeignPurchaseCosts { get; set; }
        public virtual ICollection<ForeignPurchaseAccount> ForeignPurchaseAccounts { get; set; }
        public virtual ICollection<ForeignPurchaseFollowUpItems> ForeignPurchaseFollowUpItems { get; set; }
        public virtual ICollection<ForeignPurchaseFollowUpStatus> ForeignPurchaseFollowUpStatus { get; set; }
        public virtual ICollection<ForeignPurchaseFollowUpSystemStatus> ForeignPurchaseFollowUpSystemStatus { get; set; }
        public virtual ICollection<ForeignPurchaseValidation> ForeignPurchaseValidations { get; set; }

    }
}
