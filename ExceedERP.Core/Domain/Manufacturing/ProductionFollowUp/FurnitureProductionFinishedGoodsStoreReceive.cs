using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureProductionFinishedGoodsStoreReceive: Operations
    {
        [Key]
        public int FurnitureFinishedGoodsStoreReceiveId { get; set; }
        public int FurnitureCustomerId { get; set; }
        public string VoucherNo { get; set; }
        public string StoreCode { get; set; }
        public  int FurnitureJobId { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }


        [Display(Name = "Period")]
        public int FurnitureProductionPeriodId { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Supervisor { get; set; }
        public virtual ICollection<FurnitureProductionFinishedGoodsStoreReceiveItem> ProductionFinishedGoodsStoreReceiveItems { get; set; }
        public virtual ICollection<FurnitureProductionFinishedGoodsStoreReceiveValidation> ProductionFinishedGoodsStoreReceiveValidations { get; set; } 
        public virtual ICollection<FurnitureProductionFinishedGoodsStoreReceiveDistribution> ProductionFinishedGoodsStoreReceiveDistributions { get; set; }
    }
    public class FurnitureProductionFinishedGoodsStoreReceiveItem : TrackUserOperation
    {
        [Key]
        public int FurnitureFinishedGoodsStoreReceiveItemId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public int FinishedGoodsStoreReceiveId { get; set; }
        public virtual FurnitureProductionFinishedGoodsStoreReceive PrintingProductionFinishedGoodsStoreReceive { get; set; }
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
        public decimal Balance { get; set; }
        [Display(Name = "Unit")]
        public string UOM { get; set; }

        public string AccountCode { get; set; }

        // if seed or gum
        //[Display(Name = "Processing Order Item")]
        //public int SeedProcessingOrderItemId { get; set; }
        //[Display(Name = "Processing Order Item")]
        //public int GumProcessingOrderItemId { get; set; }

    }
    public class FurnitureProductionFinishedGoodsStoreReceiveValidation : VoucherValidation
    {
        [Key]
        public int FurnitureFinishedGoodsStoreReceiveValidationId { get; set; }
        public int FurnitureFinishedGoodsStoreReceiveId { get; set; }
        public virtual FurnitureProductionFinishedGoodsStoreReceive PrintingProductionFinishedGoodsStoreReceive { get; set; }
    }
    public class FurnitureProductionFinishedGoodsStoreReceiveDistribution : VoucherDistribution
    {
        [Key]
        public int FurnitureFinishedGoodsStoreReceiveDistributionId { get; set; }
        public int FurnitureFinishedGoodsStoreReceiveId { get; set; }
        public virtual FurnitureProductionFinishedGoodsStoreReceive FurnitureProductionFinishedGoodsStoreReceive { get; set; }
        public int ReceiveItemIDTrackBack { get; set; }
    }
}
