using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public class TempGoodReceive : Operations
    {
        public int TempGoodReceiveID { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Deliverer Name")]
        public string DeliveredBy { get; set; }
        [Display(Name = "Receiver Name")]
        public string ReceivedBy { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public virtual ICollection<TempGoodReceiveItem> TempGoodReceiveItem { get; set; }
        public virtual ICollection<TempGoodReceiveValidation> TempGoodReceiveValidation { get; set; }
    }
    public class TempGoodReceiveItem : TrackUserOperation
    {
        public int TempGoodReceiveItemID { get; set; }
        public int TempGoodReceiveID { get; set; }
        public virtual TempGoodReceive TempGoodReceive { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
       // public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
    }
    public class TempGoodReceiveValidation   : TrackUserSettingOperation
    {
        public int TempGoodReceiveValidationID { get; set; }
        public int TempGoodReceiveID { get; set; }
        public virtual TempGoodReceive TempGoodReceive { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Approver { get; set; }
        public string Remark { get; set; }
    }
}