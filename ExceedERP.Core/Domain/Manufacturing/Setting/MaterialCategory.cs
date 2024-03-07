using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Setting
{
    public class ManufacturingMaterialCategory
    {
        [Key]
        public int ManufacturingMaterialCategoryId { get; set; }
        [Display(Name ="Category Code")]
        public string InventoryCategoryCode { get; set; }
        [Display(Name ="Manufacturing Category")]
        public string ManufacturingCategory { get; set; }
        [Display(Name ="Cost Account Code")]
        public string CostAccountCode { get; set; }
        [Display(Name ="Inventory Account Code")]
        public string InventoryAccountCode { get; set; }
        [Display(Name="Production Account Code")]
        public string ProductionAccountCode { get; set; }
        public bool IsActive { get; set; }

    }
    public class ManufacturingMaterialCategoryItem
    {
        [Key]
        public int ManufacturingMaterialCategoryItemId { get; set; }
        [Display(Name ="Sub-Category Code")]
        public string InventorySubCategoryCode { get; set; }
        [Display(Name ="Item Name")]
        public string ItemName { get; set; }
        [Display(Name ="Item-Code")]
        public string ItemCode { get; set; }
        [Display(Name ="UOM")]
        public string UnitOfMeasurement { get; set; }
        [Display(Name ="Unit Price")]
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        public int ManufacturingMaterialCategoryId { get; set; }

    }
}
