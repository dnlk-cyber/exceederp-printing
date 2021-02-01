using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Inventory
{
    



    public class GoodReceive : Operations
    {
        public int GoodReceiveId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Display(Name = "Receive Type")]
        public int ReceiveTypeId { get; set; }
        public virtual ReceiveType ReceiveType { get; set; }
        [Display(Name = "Purchase Reference")]
        public string ReceiveRef { get; set; }////for po and tender
        [Required]
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
        public bool IsFreshBazaar { get; set; }
        public virtual ICollection<ItemInspection> Inspections { get; set; }
        public virtual ICollection<GoodReceiveItem> GoodReceiveItem { get; set; }
        public virtual ICollection<GoodReceiveValidation> GoodReceiveValidation { get; set; }
        public virtual ICollection<GoodReceiveDistribution> GoodReceiveDistribution { get; set; }
    }

   
    public class GoodReceiveItem : TrackUserOperation
    {
        public int GoodReceiveItemID { get; set; }
        public int GoodReceiveId { get; set; }
        public virtual GoodReceive GoodReceive { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
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

    public class GoodReceiveValidation : TrackUserOperation
    {
        public int GoodReceiveValidationID { get; set; }
        public int GoodReceiveId { get; set; }
        public virtual GoodReceive GoodReceive { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }

    public class GoodReceiveDistribution:VoucherDistribution
    {
        public int GoodReceiveDistributionID { get; set; }
        public int GoodReceiveId { get; set; }
        public virtual GoodReceive GoodReceive { get; set; }
        public int GoodReceiveItemIDTrackBack { get; set; }

    }



    public class GoodReceiveDonation : Operations
    {
        public int GoodReceiveDonationId { get; set; }


        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Donated By")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier Invoice No.")]
        public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Fiscal Year")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FiscalYear { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Receive Reference")]
        public string Reference { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        public virtual ICollection<ItemInspection> Inspections { get; set; }
        public virtual ICollection<GoodReceiveDonationItem> GoodReceiveDonationItemsItem { get; set; }
        public virtual ICollection<GoodReciveDonationValidation> GoodReceivedDonationValidations { get; set; }
        public virtual ICollection<GoodReciveDonationDistribution> GoodReciveDonationDistributions { get; set; }
    }
    public class GoodReceiveDonationItem : TrackUserOperation
    {
        public int GoodReceiveDonationItemID { get; set; }
        public int GoodReceiveDonationId { get; set; }
        public virtual GoodReceiveDonation GoodReceiveDonation { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax Type")]
        [Required]
        public string TaxType { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

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
        public decimal Balance { get; set; }
        [Display(Name = "Unit")]
        public string UOM { get; set; }
        //machine
        [Display(Name = "Machinery (Tick if yes)")]
        public bool IsMachinery { get; set; }
        [Display(Name = "Batch No.")]
        public string BatchNo { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Engine Model No.")]
        public string EngineModelNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) S.No.")]
        public string EngineElectricalNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) Model No.")]
        public string EngineElectricalModelNo { get; set; }
    }

    public class GoodReciveDonationDistribution : VoucherDistribution
    {
        public int GoodReciveDonationDistributionID { get; set; }
        public int GoodReceiveDonationId { get; set; }
        public virtual GoodReceiveDonation GoodReceiveDonation { get; set; }
    }

    public class GoodReciveDonationValidation : TrackUserOperation
    {
        public int GoodReciveDonationValidationID { get; set; }
        public int GoodReceiveDonationId { get; set; }
        public virtual GoodReceiveDonation GoodReceiveDonation { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public string Remark { get; set; }
    }

    public enum GoodReceiveType
    {
        Tender,
        PurchaseOrder,
        Other
    }
    public class ReceiveType
    {
        public int ReceiveTypeId { get; set; }
        public string Name { get; set; }
        public GoodReceiveType Type { get; set; }
        public string Desctiption { get; set; }
        public bool Status { get; set; }
    }
}

