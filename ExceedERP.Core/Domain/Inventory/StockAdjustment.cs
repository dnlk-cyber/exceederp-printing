using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{

    public enum StockAdjustmentType
    {
        Shortage,
        Overage
    }
    public enum PriceAdjustmentType
    {
        Income, Loss
    }
    public class StockAdjustment:Operations
    {
        public int StockAdjustmentID { get; set; }
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Adjustment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AdjustmentDate { get; set; }
        [Required]
        [Display(Name = "Reason of Adjustment")]
        public string Reason { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public virtual ICollection<StockAdjustmentItems> StockAdjustmentItems { get; set; }
        public virtual ICollection<StockAdjustmentValidation> StockAdjustmentValidation { get; set; }
        public virtual ICollection<StockAdjustmentDistribution> StockAdjustmentDistribution { get; set; }
    }
    public class StockAdjustmentItems   : TrackUserOperation
    {
        public int StockAdjustmentItemsID { get; set; }
        public int StockAdjustmentID { get; set; }
        public virtual StockAdjustment StockAdjustment { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Balance")]
        public decimal BinCardBalance { get; set; }
        [DisplayName("Adjusted Price")]
        public decimal AdjustedPrice { get; set;  }
        [DisplayName("Stock Adjustment Type")]
        public string StockAdjustmentType { get; set;  }
        public string PriceAdjustmentType { get; set;  }
        [DisplayName("Adjusted Quantity")]
        public int AdjustedQuantity { get; set;  }
        [Display(Name = "Unit")]
        public string UOM { get; set; }
    }
    public class StockAdjustmentValidation    : TrackUserSettingOperation
    {
        public int StockAdjustmentValidationID { get; set; }
        public int StockAdjustmentID { get; set; }
        public virtual StockAdjustment StockAdjustment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Approver { get; set; }
        public string Remark { get; set; }
    }
    public class StockAdjustmentDistribution:VoucherDistribution
    {
        public int StockAdjustmentDistributionID { get; set; }
        public int StockAdjustmentID { get; set; }
        public virtual StockAdjustment StockAdjustment { get; set; }
    }

public class Revaluation :Operations
    {
        public int RevaluationId { get; set;  }
    
        [DisplayName ("Store")]
        public string StoreCode { get; set; }
        [DisplayName("Category")]

        public string ItemCategoryCode { get; set; }
        public  string Status { get; set; }
        
        public ItemCategory ItemCategory { get; set; }
        public virtual ICollection<RevaluationItem> Items { get; set;  } 
        public virtual ICollection<RevaluationValidation> Validations { get; set;  } 
        public InventoryStore Store { get; set; }

    }

   public class RevaluationItem:TrackUserSettingOperation
    {
        public int RevaluationItemId { get; set;  }
        public int RevaluationId { get; set; }

        public virtual Revaluation Revaluation { get; set; }
        [DisplayName("Stock Code")]
        public string ItemCode { get; set; }
        [DisplayName("Price")]
        public decimal PreviousPrice { get; set; }
        public decimal NewPrice { get; set;  }

        [DisplayName("Status")]
        public bool Status { get; set; }
        public string ItemCategoryCode { get; set; }
        public InventoryItem Item { get; set; }
    }

public class EvaluatedPrice
    {
    public int EvaluatedPriceId { get; set;  }
    public string ItemCode { get; set;  }
    public decimal SettedPrice { get; set;  }
    public decimal PreviousPrice { get; set;  }
    }

public class RevaluationValidation :TrackUserOperation
    {
    public int RevaluationValidationId { get; set; }
    public string Status { get; set; }
    public string Remark { get; set; }
    public int ReEvaluationId { get; set; }

    public virtual Revaluation Revaluation { get; set;  } 
    }
    
}