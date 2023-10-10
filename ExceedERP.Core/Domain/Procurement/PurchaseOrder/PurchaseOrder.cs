using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PurchaseOrder : NewOperations
    {
        public int PurchaseOrderID { get; set; }
       
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Required]
        [Display(Name = "PO Type")]
        public POType POType { get; set; }
        [Required]
        [Display(Name = "Procurement Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
       
        [Display(Name = "Purchase Req. No.")]
        public int? PurchaseRequisitionID { get; set; }
        [Display(Name = "Proforma No.")]
        public int?  ProformaPurchaseID { get; set; }
        [Display(Name = "Tend No.")]
        public int? TenderID { get; set; }
        [Display(Name = "Award No.")]
        public int? BidAwardedSupplierID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Delivery Place")]
        public string DeliveryPlace { get; set; }
        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        
        [DisplayName("Cost Center")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Procurement Officer")]
        public int ProcurementOfficer { get; set; }       
        public bool IsManual { get; set; }
        [DisplayName("SroreRequisition")]
        public string SroreRequisition { get; set; }
        [DisplayName("Purchase Status")]
        public PurchaseStatus PurchaseStatus { get; set; }
        [DisplayName("Transferred To Payment?")]
        public bool IsTransferredToPayment { get; set; }
        [DisplayName("Transferred To Followup?")]
        public bool IsTransferredToFollowUp { get; set; }
        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        [DisplayName("Is Paid")]
        public bool IsPaid { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [DisplayName("Purchase Type")]
        public PurchasePaymentType PurchasePaymentType { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItem { get; set; }
        public virtual ICollection<PurchaseOrderValidation> PurchaseOrderValidation { get; set; }
        public virtual ICollection<PurchaseOrderDistribution> PurchaseOrderDistribution { get; set; }
        public virtual ICollection<PoPaymentSchedule> PoPaymentSchedule { get; set; }
        public virtual ICollection<PoPurchaseRequisition> PoPurchaseRequisitions { get; set; }
    }

    public class PoPaymentSchedule : TrackUserOperation
    {
        public int PoPaymentScheduleId { get; set; }
        public int PurchaseOrderID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }
        [Display(Name = "Percentage")]
        public decimal Percentage { get; set; }
        public string Remark { get; set; }
    }
}
