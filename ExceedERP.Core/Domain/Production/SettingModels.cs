using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Production
{
    public class ProductionSetting : TrackUserOperation
    {
        public int ProductionSettingId { get; set; }
        public ProductionSettingType Type { get; set; }
        public string Value { get; set; }
        public string Remark { get; set; }
    }

    public class Crop : TrackUserOperation
    {
        [Key]
        public int CropId { get; set; }
        public string Name { get; set; }
        //public CropType Type { get; set; }
        //public CropGeneration Generation { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }

        [Display(Name="Item")]
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public string Description { get; set; }
    }
    public class CropViewModel
    {
        public int CropId { get; set; }
        public string Name { get; set; }
        
        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        //public ItemCategory ItemCategory { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        //public InventoryItem InventoryItem { get; set; }
        public string Description { get; set; }

        public string UoM { get; set; }

        public string OrgBranchName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class NaturalGum : TrackUserOperation
    {
        [Key]
        public int NaturalGumId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public string Description { get; set; }
    }
    public class NaturalGumViewModel
    {
        public int NaturalGumId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }

        public string Description { get; set; }
        public string UoM { get; set; }

        public string OrgBranchName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class ProductionActivity : TrackUserOperation
    {
        public int ProductionActivityId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Cycle(Stage)")]
        public int ProductionSettingId { get; set; }

        //[Display(Name = "Equipment Cost")]
        //public decimal Equipmentcost { get; set; }
        //[Display(Name = "Material Cost")]
        //public decimal MaterialCost { get; set; }
        //[Display(Name = "Labor Cost")]
        //public decimal LaborCost { get; set; }
        //[Display(Name = "Overhead Cost")]
        //public decimal OverheadCost { get; set; }
        //[Display(Name = "Profit Cost")]
        //public decimal ProfitCost { get; set; }
        //[Display(Name = "Total Cost")]
        //public decimal TotalCost { get; set; }
        public OperationType OperationType { get; set; }
        public virtual ProductionSetting ActivityType { get; set; }
    }
    public class ProductionItemCategory : TrackUserOperation
    {
        public int ProductionItemCategoryId { get; set; }

        [Display(Name="Item Category")]
        public string ItemCategoryCode { get; set; }
        public string Name { get; set; }
        //public bool IsFuel { get; set; }
        //public bool IsReinforcementBar { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public string Remark { get; set; }
    }
    public class ProductionAndProcessingCostCategory : TrackUserOperation
    {
        [Key]
        public int ProductionAndProcessingCostCategoryId { get; set; }
        public string  Title { get; set; }
        public bool HasSubCategory { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeductive { get; set; }
        public string Remark { get; set; }
    }
    public class InputMaterialApplicationRate : TrackUserOperation
    {
        [Key]
        public int InputMaterialApplicationRateId { get; set; }
        public InputMaterialApplicationPlace ApplicationPlace { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
    public class InputMaterialApplicationRateItem : TrackUserOperation
    {
        [Key]
        public int InputMaterialApplicationRateItemId { get; set; }
        public int InputMaterialApplicationRateId { get; set; }
        public virtual InputMaterialApplicationRate InputMaterialApplicationRate { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public decimal StandardRate { get; set; }
        public string Remark { get; set; }

    }
    public class InputMaterialApplicationRateItemViewModel : TrackUserOperation
    {
        public int InputMaterialApplicationRateItemId { get; set; }
        public int InputMaterialApplicationRateId { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }

        public decimal StandardRate { get; set; }
        // uom of the header item eg. Qtl
        public string HeaderUoM { get; set; }
        // uom of the the child item eg. liter ..... 
        // the concatnated display of the standard would be '10 liter per 1 Qtl'
        public string UoM { get; set; }
        public string Remark { get; set; }
    }
    public class RawProductFinishedGoodMapping : TrackUserOperation
    {
        [Key]
        public int RawProductFinishedGoodMappingId { get; set; }
        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }

        [Required]
        [Display(Name ="Raw Item")]
        public string RawItemCode { get; set; }

        [Required]
        [Display(Name = "By-Product")]
        public string ByproductItemCode { get; set; }

        [Required]
        [Display(Name = "Cleaned Item")]
        public string CleanedItemCode { get; set; }

        public string Remark { get; set; }
    }
}
