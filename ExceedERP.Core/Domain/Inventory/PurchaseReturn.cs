using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public class PurchaseReturn    : Operations
    {
        public int PurchaseReturnID { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Required]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "Receive Reference")]
        public int GoodReceiveId { get; set; }
        public virtual GoodReceive GoodReceive { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public virtual ICollection<PurchaseReturnItems> PurchaseReturnItems { get; set; }
        public virtual ICollection<PurchaseReturnValidation> PurchaseReturnValidation { get; set; }
        public virtual ICollection<PurchaseReturnDistribution> PurchaseReturnDistribution { get; set; }
    }
    public class PurchaseReturnItems : TrackUserOperation
    {
        public int PurchaseReturnItemsID { get; set; }
        public int PurchaseReturnID { get; set; }
        public virtual PurchaseReturn PurchaseReturn { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }
    public class PurchaseReturnValidation  : TrackUserSettingOperation
    {
        public int PurchaseReturnValidationID { get; set; }
        public int PurchaseReturnID { get; set; }
        public virtual PurchaseReturn PurchaseReturn { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class PurchaseReturnDistribution: VoucherDistribution
    {
        public int PurchaseReturnDistributionID { get; set; }
        public int PurchaseReturnID { get; set; }
        public virtual PurchaseReturn PurchaseReturn { get; set; }
    }
}