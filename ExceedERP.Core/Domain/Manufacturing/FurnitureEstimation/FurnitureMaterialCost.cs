using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureMaterialCost : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureMaterialCostId { get; set; }
        public int FurnitureEstimationId { get; set; }
        public FurnitureEstimationForm Estimation { get; set; }
        [Display(Name = "Inventory Category")]
        public string InventoryCategoryId { get; set; }
        [Display(Name = "Inventory")]
        public string InventoryId { get; set; }
        public int? JobId { get; set; }
        public string StoreCode { get; set; }

        public string UnitOfMeasurment { get; set; }

        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [ReadOnly(true)]
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public decimal Balance { get; set; }
        [Display(Name ="Task Type")]
        public WorkShop TaskType { get; set; }
        [Display(Name = "Material Category")]
        public int ManufacturingMaterialCategoryId { get; set; }
        [Display(Name ="Item")]
        public int ManufacturingMaterialCategoryItemId { get; set; }

    }
}
