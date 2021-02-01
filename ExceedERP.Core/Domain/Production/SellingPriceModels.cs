using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class CleanedProductSellingPriceBase : TrackUserOperation
    {
        [Display(Name ="Raw")]
        public decimal RawQuantity { get; set; }
        [Display(Name = "Cleaned")]
        public decimal CleanedQuantity { get; set; }

        [Display(Name = "Production")]
        public decimal ProductionCost { get; set; }
        [Display(Name = "Processing & Packing")]
        public decimal ProcessingAndPackingCost { get; set; }
        public decimal Recovery { get; set; }
        public decimal TotalNetCost { get; set; }
        //public decimal SellingPrice { get; set; }
        //public decimal ApprovedSellingPrice { get; set; }

        //[DisplayName("Item Category")]
        //public string ItemCategoryCode { get; set; }
        //public virtual ItemCategory ItemCategory { get; set; }

        //[DisplayName("Item Type")]
        //public string ItemCode { get; set; }
        //public virtual InventoryItem InventoryItem { get; set; }
    }
    public class CleanedSeedSellingPrice : TrackUserOperation
    {
        [Key]
        public int CleanedSeedSellingPriceId { get; set; }
        [Display(Name ="Prodution Year")]
        public int ProductionYearId { get; set; }
        public ProductionYear ProductionYear { get; set; }
        [Display(Name ="Crop")]
        public int CropId { get; set; }
        public virtual Crop Crop { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ApprovedSellingPrice { get; set; }
    }
   public class CleanedSeedSellingPriceEditViewModel
    {
        public int CleanedSeedSellingPriceId { get; set; }
        [Display(Name ="Calculated Selling Price")]
        public decimal SellingPrice { get; set; }
        public decimal ApprovedSellingPrice { get; set; }
        public string Crop { get; set; }
        public string UoM { get; set; }
    }
    public class CleanedSeedSellingPriceItem : CleanedProductSellingPriceBase
    {
        [Key]
        public int CleanedSeedSellingPriceItemId { get; set; }

        [Display(Name ="Processing Order")]
        public int SeedProcessingOrderItemId { get; set; }
        public virtual SeedProcessingOrderItem SeedProcessingOrderItem { get; set; }
        public int CleanedSeedSellingPriceId { get; set; }
        public virtual CleanedSeedSellingPrice CleanedSeedSellingPrice { get; set; }
    }
    public class CleanedNaturalGumSellingPrice : TrackUserOperation
    {
        [Key]
        public int CleanedNaturalGumSellingPriceId { get; set; }

        public int ProductionYearId { get; set; }
        public ProductionYear ProductionYear { get; set; }
        public int NaturalGumId { get; set; }
        public virtual NaturalGum NaturalGum { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ApprovedSellingPrice { get; set; }
    }
    public class CleanedNaturalGumSellingPriceItem : CleanedProductSellingPriceBase
    {
        [Key]
        public int CleanedNaturalGumSellingPriceItemId { get; set; }

        [Display(Name = "Processing Order")]
        public int GumProcessingOrderItemId { get; set; }
        public virtual GumProcessingOrderItem GumProcessingOrderItem { get; set; }

        public int CleanedNaturalGumSellingPriceId { get; set; }
        public virtual CleanedNaturalGumSellingPrice CleanedNaturalGumSellingPrice { get; set; }
    }
}
