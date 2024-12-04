
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingMaterialCategory
    {
        [Key]
        public int PrintingMaterialCategoryId { get; set; }
        [Display(Name = "Category Code")]
        public string InventoryCategoryCode { get; set; }
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Display(Name = "Cost Account Code")]
        public string CostAccountCode { get; set; }
        [Display(Name = "Inventory Account Code")]
        public string InventoryAccountCode { get; set; }
        [Display(Name = "Production Account Code")]
        public string ProductionAccountCode { get; set; }
        public bool IsActive { get; set; }
    }
    public class PrintingMaterialCategoryItem 
    {
        [Key]
        public int PrintingMaterialCategoryItemId { get; set; } 
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
        public double Width { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }


        //Fk
        public int PrintingMaterialCategoryId { get; set; }

    }
}
