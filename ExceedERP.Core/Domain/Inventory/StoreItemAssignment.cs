using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{
    public enum InventoryBalanceTypes
    {
        Standard,
        Begining,
        Ending
    }

    public enum BinCardTransactionTypes
    {
        NA,
        Issue,
        Receive,
        TransferSend,
        TransferReceive,
        Return,
        Adjustment,
        SalesOrLend,
        Count,
    }

    public enum BinCardTransactionStatus
    {
        NA,
        Regular,
        Void,
    }

    public class StoreItemAssignment : TrackUserSettingOperation
    {
        public int StoreItemAssignmentID { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        public virtual InventoryStore InventoryStore { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Safety Point")]
        public decimal SafetyPoint { get; set; }
        [Display(Name = "Danger Point")]
        public decimal DangerPoint { get; set; }
        [Display(Name = "Reorder Point")]
        public decimal ReOrderPoint { get; set; }
        [Display(Name = "Minimum Point")]
        public decimal MinPoint { get; set; }
        [Display(Name = "Maximum Point")]
        public decimal MaxPoint { get; set; }
        public decimal Balance { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal BegBalance { get; set; }
        public decimal BegAvgPrice { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        public InventoryBalanceTypes Type { get; set; }
      

        public string ItemLocation { get; set; }

        public string Remark { get; set; }
        public virtual ICollection<BinCardLine> BinCardLines { get; set; }
        
    }

    public class BinCardLine: BinCardLineBase
    {
        public int BinCardLineID { get; set; }
        public int StoreItemAssignmentID { get; set; }
        public virtual StoreItemAssignment StoreItemAssignment { get; set; }      
        

    }

    
}
