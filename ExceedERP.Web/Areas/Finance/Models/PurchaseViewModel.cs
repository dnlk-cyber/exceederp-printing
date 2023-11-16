using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Web.Areas.Finance.Models
{
    public class PurchaseViewModel : Operations
    {
        public int PurchaseId { get; set; }
        public string TenderRef { get; set; }
        public int? EmployeeId { get; set; }
        [Display(Name = "Purchase Source")]
        public PurchaseType PoTypeEnum { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        
        [Display(Name = "Procurement Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
       
        [Display(Name = "Purchase Req. No.")]
        public int? PurchaseRequisitionID { get; set; }
        [Display(Name = "Proforma No.")]
        public int?  ProformaPurchaseID { get; set; }
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
        [Display(Name = "Purchaser")]
        public int ProcurementOfficer { get; set; }

        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        public bool IsManual { get; set; }
        [DisplayName("SroreRequisition")]
        public string SroreRequisition { get; set; }
        public virtual ICollection<PurchaseItemViewModel> PurchaseViewModelItem { get; set; }
        public virtual ICollection<PurchaseValidationViewModel> PurchaseViewModelValidation { get; set; }
        public virtual ICollection<PurchasePaymentScheduleViewModel> PoPaymentSchedule { get; set; }
    }
    public class PurchaseItemViewModel : TrackUserOperation
    {
        public int PurchaseItemViewModelId { get; set; }
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        public string Remark { get; set; }
    }
    public class PurchaseValidationViewModel : TrackUserOperation
    {
        public int PurchaseValidationViewModelId{ get; set; }
       
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class PurchasePaymentScheduleViewModel : TrackUserOperation
    {
        public int PurchasePaymentScheduleViewModelId { get; set; }
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }
        public AmountType AmountType { get; set; }
        [Display(Name = "Amount")]
        public decimal Percentage { get; set; }
        public string Remark { get; set; }
    }
}
