using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
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
        [Display(Name = "UoM")]
        public string UnitOfMeasurment { get; set; }
        [Display(Name ="Quantity")]
        public decimal Quantity { get; set; }      
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; }
        
        public int PrintingCostEstimationId { get; set; }

        [Display(Name ="Material Type")]
        public EstimationMaterialType EstimationMaterialType { get; set; }
        public double WastageInPercentage { get; set; }
        
    }
    public enum EstimationMaterialType
    {
        [Display(Name ="Text Paper"), Description("Text Paper")]
        TextPaper,
        [Display(Name = "Cover Paper"), Description("Cover Paper")]
        CoverPaper,
        [Display(Name = "Text Plate"), Description("Text Paper")]
        TextPlate,
        [Display(Name = "Cover Plate"), Description("Cover Plate")]
        CoverPlate,
        [Display(Name = "Wrapping Paper"), Description("Wrapping Paper")]
        WrappingPaper,
        [Display(Name ="Glue")]
        Glue,
        [Display(Name ="Stitching Wire")]
        StitchingWire,
        [Display(Name ="Ink Text")]
        InkText,
        [Display(Name = "Ink Cover")]
        InkCover

    }
}
