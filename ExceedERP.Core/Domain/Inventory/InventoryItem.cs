using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;

namespace ExceedERP.Core.Domain.Inventory
{
    public enum StockCostTypes
    {
        WeightedAverage, FIFO, LIFO
    }
    public class UoM:TrackUserSettingOperation
    {

        [Required]
        [Key]
        [DisplayName("Name")]
        public string UoMName { get; set; }
        public string Description { get; set; }
        public bool Default { get; set; }
    }
    public class ItemCategory:TrackUserSettingOperation
    {

        [Required]
        [Key]
        [Display(Name = "Code")]
        public string ItemCategoryCode { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Required]
        [DisplayName("Issue Account")]
        public string IssueAccountCode { get; set; }
        public bool HaveParent { get; set; }
        [DisplayName("Is Fixed Asset")]
        public bool IsFixedAsset { get; set; }
        public string ParentName { get; set; }
        public string Abbreviation { get; set; }
        public StockCostTypes CostType { get; set; }

        public virtual ICollection<InventoryItem> Items { get; set; }
        public virtual ICollection<ItemCategoryUserRole> Roles { get; set; }
    }

    public enum ItemTypes
    {
        FixedAsset, StockCommonItem, Service
    }
    public class InventoryItem:TrackUserSettingOperation
    {
        [Required]
        [Key]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        //[Required]
        public ItemTypes Type { get; set; }
        [DisplayName("Unit")]
        public string UoMName { get; set; }
        public virtual UoM UoM { get; set; }
        public decimal Balance { get; set; }
        public decimal AvgPrice { get; set; }
        public bool HasTransaction { get; set; }
        public bool Enable { get; set; }
        public string PartNo { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        public string Prefix { get; set; } //category+sub prefix
        public int ItemIndex { get; set; } //sequencial number within category
        public string SubCategoryCode { get; set; }

        public virtual ICollection<StoreItemAssignment> StoreItemAssignments { get; set; }

    }

    public class ItemCategoryUserRole : TrackUserSettingOperation
    {
        
        public int ItemCategoryUserRoleId { get; set; }
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public string RoleName { get; set; }
        public string Remark { get; set; }
    }


    public class ItemPerStoreBalance
    {
        public int ItemPerStoreBalanceID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string UoMName { get; set; }
        public decimal Balance { get; set; }
        public DateTime AvgPrice { get; set; }
        public string Store { get; set; }
        public virtual UoM UoM { get; set; }

    }

    public class Machinary
    {
        public int MachinaryId { get; set; }
        public string ItemCode { get; set; }
        [Display(Name = "Machinery (Tick if yes)")]
        public bool IsMachinery { get; set; }
        [Display(Name = "Batch No.")]
        public string BatchNo { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Engine Model No.")]
        public string EngineModelNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) S.No.")]
        public string EngineElectricalNo { get; set; }
        [Display(Name = "Electrical Part (Body Part) Model No.")]
        public string EngineElectricalModelNo { get; set; }

    }


}
