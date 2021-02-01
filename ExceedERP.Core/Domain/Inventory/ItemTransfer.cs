using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;

namespace ExceedERP.Core.Domain.Inventory
{
    public enum Driver
    {
       Internal , External
    }
    public class ItemTransfer  : Operations
    {
        public int ItemTransferID { get; set; }
        [Display(Name = "Requisition Ref No.")]
        public int StoreRequisitionID { get; set; }
        [DisplayName("Document Ref No.")]
        public string DocumentRef { get; set; }
        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransferDate { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReceiveDate { get; set; }
        [Required]
        [Display(Name = "Ship From")]
        public string FromStore { get; set; }
        [Required]
        [Display(Name = "Ship To")]
        public string ToStore { get; set; }
        [Display(Name = "Deliverer Name")]
        public string DeliveredBy { get; set; }
        [Display(Name = "Vehicle Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Deliverer Type")]
        public Driver DriverType { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public bool IsSecondVoid { get; set; }
        public string SecondVoidBy { get; set; }
        public DateTime? SecondVoidTime { get; set; }
        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        public virtual ICollection<ItemTransferItem> ItemTransferItem { get; set; }
        public virtual ICollection<ItemTransferValidation> ItemTransferValidation { get; set; }
        public virtual ICollection<ItemTransferReciverValidation> ItemTransferReciverValidations { get; set; } 
        
        public virtual ICollection<ItemTransferDistribution> ItemTransferDistribution { get; set; }       
    }
    public class ItemTransferItem : TrackUserOperation
    {
        public int ItemTransferItemID { get; set; }
        public int ItemTransferID { get; set; }
        public virtual ItemTransfer ItemTransfer { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Part/Code No.")]
        public string PartOrCodeNumber { get; set; }
        [Display(Name = "Requested Quantity")]
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "Actually Shipped")]
        public decimal ShippedQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UPrice { get; set; }
        [Display(Name = "Total Price")]
        public decimal TPrice { get; set; }
        [Display(Name = "Received Quantity")]
        public decimal ReceivedQuantity { get; set; }
        public string Remark { get; set; }
         
    }
    public class ItemTransferValidation    :TrackUserSettingOperation
    {
        public int ItemTransferValidationID { get; set; }
        public int ItemTransferID { get; set; }
        
        public virtual ItemTransfer ItemTransfer { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }  
    public class ItemTransferReciverValidation    :TrackUserSettingOperation
    {
        public int ItemTransferReciverValidationID { get; set; }
        public int ItemTransferID { get; set; }
        public virtual ItemTransfer ItemTransfer { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
   
    public class ItemTransferDistribution:VoucherDistribution
    {
        public int ItemTransferDistributionID { get; set; }
        public int ItemTransferID { get; set; }
        public virtual ItemTransfer ItemTransfer { get; set; }

    }
}