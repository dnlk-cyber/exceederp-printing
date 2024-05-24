using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingEstimationMaterialCost : TrackUserSettingOperation
    {
        [Key]
        public int PrintingEstimationMaterialCostId { get; set; }
        [Display(Name = "Item Category")]
        public int PrintingMaterialCategoryId { get; set; }
        [Display(Name = "Item Name")]
        public int PrintingMaterialCategoryItemId { get; set; }

        public decimal Quantity { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; }
        [Display(Name ="UoM")]
        public string UnitOfMeasurment { get; set; }
        public int PrintingCostEstimationId { get; set; } 
    }
}
