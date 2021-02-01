using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.Procurement.Models
{

    public enum GrnType
    {
        Stock,FixedAsset,Service
    }


    public class GrnViewModel : Operations
    {
        public GrnType GrnType { get; set; }
        public int GrnId { get; set; }
       
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Display(Name = "Receive Type")]
        public int ReceiveTypeId { get; set; }
        public virtual ReceiveType ReceiveType { get; set; }
        [Display(Name = "Purchase Reference")]
        public string ReceiveRef { get; set; }////for po and tender
       
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
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
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public bool IsManual { get; set; }
        public virtual ICollection<GrnViewModelItem> GrnViewModelItem { get; set; }
        public virtual ICollection<GrnViewModelValidation> GrnViewModelValidation { get; set; }
    }

   
    public class GrnViewModelItem : TrackUserOperation
    {
        public int GrnViewModelItemID { get; set; }
        public virtual GrnViewModel GrnViewModel { get; set; }
       
        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Sub Category")]
        public string SubCategory { get; set; }

        [DisplayName("Item ")]
        public string Item { get; set; }
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
        [Display(Name = "Tick if Extra Quantity")]
        public bool IsExtra { get; set; }
        [Display(Name = "Extra Quantity")]
        public decimal? ExtraQuantity { get; set; }
        public decimal ExtraAmount { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Warranty Information")]
        public string WarrantyInformation { get; set; }
        [DataType(DataType.MultilineText)]
        public string Specification { get; set; }
        [Display(Name = "This item expires")]
        public bool Expire { get; set; }
        [Display(Name = "Expire Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpireDate { get; set; }
        public decimal Balance { get; set; } //FIFO LIFO
        
        [Display(Name = "Unit")]
        public string UOM { get; set; }

    }

    public class GrnViewModelValidation : TrackUserOperation
    {
        public int GrnViewModelValidationID { get; set; }
        public virtual GrnViewModel GrnViewModel { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }

   
   
}

