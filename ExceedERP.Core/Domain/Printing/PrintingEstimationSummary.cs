using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingEstimationSummary : Operations
    {
        [Key]
        public int PrintingEstimationSummaryId { get; set; }
        public int CustomerId { get; set; }
        public bool IsClosed { get; set; }
        public bool IsCalculated { get; set; }
        // FK
        public int PrintingCostEstimationId { get; set; }
    }
    public class PrintingEstimationDetail
    {
        [Key]
        public int PrintingEstimationDetailId { get; set; }
        public int JobTypeId { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Material Cost")]
        public decimal MaterialCost { get; set; }
        [Display(Name = "Direct labour Cost")]
        public decimal LabourCost { get; set; }
        public decimal GraphicsCost { get; set; }
        [Display(Name = "OverHead Cost(%)")]
        [Required]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Manuf. Cost")]
        public decimal ManufacturingCost { get; set; }
        [Display(Name = "Admin Cost(%)")]
        [Required]
        public decimal AdministrativeCost { get; set; }
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCalculated { get; set; }
        //Fk
        public int PrintingEstimationSummaryId { get; set; }

    }
    public class PrintingEstimationMargin
    {
        [Key]
        public int PrintingEstimationMarginId { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsApprovedMargin { get; set; }

        //
        public int PrintingEstimationDetailId { get; set; }

    }
    public class PrintingEstimationCostSummary
    {
        [Key]
        public int PrintingEstimationCostSummaryId { get; set; }
        public decimal CommulativeCost { get; set; }
        [Display(Name = "Profit Margin(%)")]
        [Required]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Final Price")]
        public decimal FinalPriceIncludingProfit { get; set; }
        public bool IsApprovedMargin { get; set; }
        //Fk
        public int PrintingEstimationSummaryId { get; set; } 

    }
}
