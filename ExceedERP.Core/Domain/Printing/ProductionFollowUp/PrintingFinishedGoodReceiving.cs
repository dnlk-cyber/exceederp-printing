using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
   public class PrintingProductionFinishedGoodsStoreReceive: Operations
    {
        [Key]
        public int FinishedGoodsStoreReceiveId { get; set; }
        public int CustomerId { get; set; }
        public string VoucherNo { get; set; }
        public string StoreCode { get; set; }
        public  int JobId { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }


        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Supervisor { get; set; }
        public virtual ICollection<PrintingProductionFinishedGoodsStoreReceiveItem> PrintingProductionFinishedGoodsStoreReceiveItems { get; set; } = new List<PrintingProductionFinishedGoodsStoreReceiveItem>();
        public virtual ICollection<PrintingProductionFinishedGoodsStoreReceiveValidation> PrintingProductionFinishedGoodsStoreReceiveValidations { get; set; } = new List<PrintingProductionFinishedGoodsStoreReceiveValidation>();
        public virtual ICollection<PrintingProductionFinishedGoodsStoreReceiveDistribution> PrintingProductionFinishedGoodsStoreReceiveDistributions { get; set; } = new List<PrintingProductionFinishedGoodsStoreReceiveDistribution>();
    }
    public class PrintingProductionFinishedGoodsStoreReceiveItem : TrackUserOperation
    {
        [Key]
        public int FinishedGoodsStoreReceiveItemId { get; set; }

        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public int FinishedGoodsStoreReceiveId { get; set; }
        public virtual PrintingProductionFinishedGoodsStoreReceive PrintingProductionFinishedGoodsStoreReceive { get; set; }
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
    public class PrintingProductionFinishedGoodsStoreReceiveValidation : VoucherValidation
    {
        [Key]
        public int FinishedGoodsStoreReceiveValidationId { get; set; }
        public int FinishedGoodsStoreReceiveId { get; set; }
        public virtual PrintingProductionFinishedGoodsStoreReceive PrintingProductionFinishedGoodsStoreReceive { get; set; }
    }
    public class PrintingProductionFinishedGoodsStoreReceiveDistribution : VoucherDistribution
    {
        [Key]
        public int FinishedGoodsStoreReceiveDistributionId { get; set; }
        public int FinishedGoodsStoreReceiveId { get; set; }
        public virtual PrintingProductionFinishedGoodsStoreReceive PrintingProductionFinishedGoodsStoreReceive { get; set; }
        public int ReceiveItemIDTrackBack { get; set; }
    }
}
