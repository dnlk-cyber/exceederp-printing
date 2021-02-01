using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Inventory
{

    public class ItemTransferReceive : Operations
    {
        public int ItemTransferReceiveId { get; set; }
        [Display(Name = "Transfer No.")]
        public int ItemTransferID { get; set; }
        [DisplayName("Document Ref No.")]
        public string DocumentRef { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
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
        public string DriverType { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public int BranchId { get; set; }
        public virtual ICollection<ItemTransferItemReceive> ItemTransferItemReceive { get; set; }
        public virtual ICollection<ItemTransferReceiverValidation> ItemTransferReceiverValidation { get; set; }
        public virtual ICollection<ItemTransferReceiveDistribution> ItemTransferReceiveDistribution { get; set; }
    }
    public class ItemTransferItemReceive : TrackUserOperation
    {
        public int ItemTransferItemReceiveId { get; set; }
        public int ItemTransferReceiveId { get; set; }
        public virtual ItemTransferReceive ItemTransferReceive { get; set; }
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
        [Display(Name = "Received Quantity")]
        public decimal ReceivedQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UPrice { get; set; }
        [Display(Name = "Total Price")]
        public decimal TPrice { get; set; }
        public string Remark { get; set; }

    }
    
    public class ItemTransferReceiverValidation : TrackUserSettingOperation
    {
        public int ItemTransferReceiverValidationId { get; set; }
        public int ItemTransferReceiveId { get; set; }
        public virtual ItemTransferReceive ItemTransferReceive { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }

    public class ItemTransferReceiveDistribution:VoucherDistribution
    {
        public int ItemTransferReceiveDistributionId { get; set; }
        public int ItemTransferReceiveId { get; set; }
        public virtual ItemTransferReceive ItemTransferReceive { get; set; }
    }
}