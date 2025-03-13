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
        [Display(Name ="Weight")]
        public decimal PaperWeight { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; }
        [Display(Name ="UoM")]
        public string UnitOfMeasurment { get; set; }
        public int PrintingCostEstimationId { get; set; }

        [Display(Name ="Material Type")]
        public EstimationMaterialType EstimationMaterialType { get; set; }
        public double WastageInPercentage { get; set; }
    }
    public enum EstimationMaterialType
    {
        TextPaper,
        CoverPaper,
        TextPlate,
        CoverPlate,
        Ink,
        Ink_100CoverageOnA4Cover,
        Ink_Other,
        StitchingWire,
        Sewing_Thread,
        HotMelt_Glue,
        Wrapping_Paper
    }
}
