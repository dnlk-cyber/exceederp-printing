using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public enum SellorLends
    {
        Donate,Sell , Lend
    }
    public class GoodSellingAndLending  :Operations
    {
        public int GoodSellingAndLendingID { get; set; }
        [Required]
        [Display(Name = "Sell or Lend")]
        public string SellOrLend { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Required]
        [Display(Name = "Organization Name")]
        public string Buyer { get; set; }
        [Required]
        [Display(Name = "Organization's Address")]
        public string BuyerAddress { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public virtual ICollection<GoodSellingAndLendingItem> GoodSellingAndLendingItems { get; set; }
        public virtual ICollection<GoodSellingAndLendingValidation> GoodSellingAndLendingValidation { get; set; }
        public virtual ICollection<GoodSellingAndLendingDistribution> GoodSellingAndLendingDistribution { get; set; }
    }
    public class GoodSellingAndLendingItem :TrackUserOperation
    {
        public int GoodSellingAndLendingItemID { get; set; }
        public int GoodSellingAndLendingID { get; set; }
        public virtual GoodSellingAndLending GoodSellingAndLending { get; set; }
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Unit")]
        public string UOM { get; set; }
        [Display(Name = "Quantity")]
        public decimal QuantitySold { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPurchasingPrice { get; set; }
        [Display(Name = "Unit Selling Price")]
        public decimal UnitSellingPrice { get; set; }
        [Display(Name = "Total Selling Price")]
        public decimal TotalSellingPrice { get; set; }
      
        [Display(Name = "GL Account")]
        public string GLAccountCode { get; set; }
       
        

    }
    public class GoodSellingAndLendingValidation :VoucherValidation
    {
        public int GoodSellingAndLendingValidationID { get; set; }
        public int GoodSellingAndLendingID { get; set; }
        public virtual GoodSellingAndLending GoodSellingAndLending { get; set; }
    }
    public class GoodSellingAndLendingDistribution:VoucherDistribution
    {
        public int GoodSellingAndLendingDistributionID { get; set; }
        public int GoodSellingAndLendingID { get; set; }
        public virtual GoodSellingAndLending GoodSellingAndLending { get; set; }        
    }
}