using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement
{


    public class ServiceReceive : Operations
    {

        public int ServiceReceiveId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }

        [Display(Name = "Receive Type")]
        public int ReceiveTypeId { get; set; }
        public virtual ReceiveType ReceiveType { get; set; }
        [Display(Name = "Purchase Reference")]
        public string ReceiveRef { get; set; }////for po and tender
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }//for po and tender
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }//for manual
        [Display(Name = "Supplier Invoice No.")]
        public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Manual Reference")] // manual reference
        public string Reference { get; set; }

        [Display(Name = "Lot")]
        public string TenderLot { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Receive Date")]
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public PurchaseItemType ItemType { get; set; }
        public virtual ICollection<ServiceReceiveItem> ServiceReceiveItems { get; set; }
        public virtual ICollection<ServiceReceiveValidation> ServiceReceiveValidation { get; set; }
        public virtual ICollection<ServiceReceiveDistribution> ServiceReceiveDistribution { get; set; }
    }
    public class ServiceReceiveItem : TrackUserSettingOperation
    {
        public int ServiceReceiveItemId { get; set; }
        public int ServiceReceiveId { get; set; }
        public virtual ServiceReceive ServiceReceive { get; set; }
       
        [DisplayName("Item Category Code")]
        public string ServiceCategoryCode { get; set; }
        [DisplayName("Item Category Name")]
        public string ServiceCategoryName { get; set; }
        [DisplayName("Item Code")]
        public string ServiceItemCode { get; set; }
        [DisplayName("Item Name")]
        public string ServiceItemName { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        public PurchaseItemType ItemType { get; set; }

    }

    public class ServiceReceiveValidation : TrackUserSettingOperation
    {
        public int ServiceReceiveValidationId { get; set; }
        public int ServiceReceiveId { get; set; }

        public virtual ServiceReceive ServiceReceive { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class ServiceReceiveDistribution: VoucherDistribution
    {
        public int ServiceReceiveDistributionId { get; set; }
        public int ServiceReceiveId { get; set; }

        public virtual ServiceReceive ServiceReceive { get; set; }
    }
}