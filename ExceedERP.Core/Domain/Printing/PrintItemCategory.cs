using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintItemCategory
    {
        public int PrintItemCategoryId { get; set; }
        [Display(Name = "Category Code")]
        public string InventoryCategoryCode { get; set; }
        [Display(Name = "Manufacturing Category")]
        public string PrintCategory { get; set; }
        [Display(Name = "Cost Account Code")]
        public string CostAccountCode { get; set; }
        [Display(Name = "Inventory Account Code")]
        public string InventoryAccountCode { get; set; }
        [Display(Name = "Production Account Code")]
        public string ProductionAccountCode { get; set; }
        public bool IsActive { get; set; }
    }
    public class PrintInventoryItem
    {
        [Key]
        public int PrintInventoryItemId { get; set; }
        [Display(Name = "Sub-Category Code")]
        public string InventorySubCategoryCode { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Item-Code")]
        public string ItemCode { get; set; }
        [Display(Name = "UOM")]
        public string UnitOfMeasurement { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        [Display(Name ="Item Desc.")]
        public string ItemDescription { get; set; }
        [Display(Name = "Min level")]
        public int MinLevel { get; set; }
        [Display(Name = "Max level")]
        public int MaxLevel { get; set; }
        [Display(Name = "Alt part number")]
        public string AltPartNumber { get; set; }

        //FK
        public int ManufacturingMaterialCategoryId { get; set; }

    }
}
