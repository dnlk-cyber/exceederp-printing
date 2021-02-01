using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class ProductionStoreReceive : Operations
    {
        [Key]
        public int ProductionStoreReceiveId { get; set; }

        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }

        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }

        public ProductType ProductType { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        public virtual ICollection<ProductionStoreReceiveItem> ProductionStoreReceiveItems { get; set; } = new List<ProductionStoreReceiveItem>();
        public virtual ICollection<ProductionStoreReceiveValidation> ProductionStoreReceiveValidations { get; set; } = new List<ProductionStoreReceiveValidation>();
        public virtual ICollection<ProductionStoreReceiveDistribution> ProductionStoreReceiveDistributions { get; set; } = new List<ProductionStoreReceiveDistribution>();
    }
    public class ProductionStoreReceiveItem : TrackUserOperation
    {
        [Key]
        public int ProductionStoreReceiveItemId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public int ProductionStoreReceiveId { get; set; }
        public virtual ProductionStoreReceive ProductionStoreReceive { get; set; }
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
        //[Display(Name = "Tick if Extra Quantity")]
        //public bool IsExtra { get; set; }
        //[Display(Name = "Extra Quantity")]
        //public decimal? ExtraQuantity { get; set; }
        //public decimal ExtraAmount { get; set; }
        //[DataType(DataType.MultilineText)]
        //[Display(Name = "Warranty Information")]
        //public string WarrantyInformation { get; set; }
        //[DataType(DataType.MultilineText)]
        //public string Specification { get; set; }
        //[Display(Name = "This item expires")]
        //public bool Expire { get; set; }
        //[Display(Name = "Expire Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime? ExpireDate { get; set; }
        public decimal Balance { get; set; }
        [Display(Name = "Unit")]
        public string UOM { get; set; }

        public string AccountCode { get; set; }

        // if seed or gum
        [Display(Name = "Production Order Item")]
        public int SeedProductionOrderItemId { get; set; }
        [Display(Name = "Production Order Item")]
        public int GumProductionOrderItemId { get; set; }

    }
    public class ProductionStoreReceiveValidation : VoucherValidation
    {
        [Key]
        public int ProductionStoreReceiveValidationId { get; set; }
        public int ProductionStoreReceiveId { get; set; }
        public virtual ProductionStoreReceive ProductionStoreReceive { get; set; }
    }
    public class ProductionStoreReceiveDistribution : VoucherDistribution
    {
        public int ProductionStoreReceiveDistributionId { get; set; }
        public int ProductionStoreReceiveId { get; set; }
        public virtual ProductionStoreReceive ProductionStoreReceive { get; set; }
        public int ReceiveItemIDTrackBack { get; set; }
    }
    public class ProductionFinishedGoodsStoreReceive : Operations
    {
        [Key]
        public int ProductionFinishedGoodsStoreReceiveId { get; set; }

        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }

        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }

        public ProductType ProductType { get; set; }


        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        public virtual ICollection<ProductionFinishedGoodsStoreReceiveItem> ProductionFinishedGoodsStoreReceiveItems { get; set; } = new List<ProductionFinishedGoodsStoreReceiveItem>();
        public virtual ICollection<ProductionFinishedGoodsStoreReceiveValidation> ProductionFinishedGoodsStoreReceiveValidations { get; set; } = new List<ProductionFinishedGoodsStoreReceiveValidation>();
        public virtual ICollection<ProductionFinishedGoodsStoreReceiveDistribution> ProductionFinishedGoodsStoreReceiveDistributions { get; set; } = new List<ProductionFinishedGoodsStoreReceiveDistribution>();


    }
    public class ProductionFinishedGoodsStoreReceiveItem : TrackUserOperation
    {
        [Key]
        public int ProductionFinishedGoodsStoreReceiveItemId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public int ProductionFinishedGoodsStoreReceiveId { get; set; }
        public virtual ProductionFinishedGoodsStoreReceive ProductionFinishedGoodsStoreReceive { get; set; }
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
        //[Display(Name = "Tick if Extra Quantity")]
        //public bool IsExtra { get; set; }
        //[Display(Name = "Extra Quantity")]
        //public decimal? ExtraQuantity { get; set; }
        //public decimal ExtraAmount { get; set; }
        //[DataType(DataType.MultilineText)]
        //[Display(Name = "Warranty Information")]
        //public string WarrantyInformation { get; set; }
        //[DataType(DataType.MultilineText)]
        //public string Specification { get; set; }
        //[Display(Name = "This item expires")]
        //public bool Expire { get; set; }
        //[Display(Name = "Expire Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime? ExpireDate { get; set; }
        public decimal Balance { get; set; }
        [Display(Name = "Unit")]
        public string UOM { get; set; }

        public string AccountCode { get; set; }

        // if seed or gum
        [Display(Name = "Processing Order Item")]
        public int SeedProcessingOrderItemId { get; set; }
        [Display(Name = "Processing Order Item")]
        public int GumProcessingOrderItemId { get; set; }

    }
    public class ProductionFinishedGoodsStoreReceiveValidation : VoucherValidation
    {
        [Key]
        public int ProductionFinishedGoodsStoreReceiveValidationId { get; set; }
        public int ProductionFinishedGoodsStoreReceiveId { get; set; }
        public virtual ProductionFinishedGoodsStoreReceive ProductionFinishedGoodsStoreReceive { get; set; }
    }
    public class ProductionFinishedGoodsStoreReceiveDistribution : VoucherDistribution
    {
        [Key]
        public int ProductionFinishedGoodsStoreReceiveDistributionId { get; set; }
        public int ProductionFinishedGoodsStoreReceiveId { get; set; }
        public virtual ProductionFinishedGoodsStoreReceive ProductionFinishedGoodsStoreReceive { get; set; }
        public int ReceiveItemIDTrackBack { get; set; }
    }
}
