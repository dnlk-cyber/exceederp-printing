using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class EstimationSummary : Operations
    {
        [Key]
        public int EstimationSummaryId { get; set; }
        public int CustomerId { get; set; }
        public bool IsClosed { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsMarginApproved { get; set; } //to hide and show the Approve button
        //Fk
        public int FurnitureEstimationId { get; set; }

    }

    public class EstimationDetail
    {
        [Key]
        public int EstimationDetailId { get; set; }      
        public int JobTypeId { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Material Cost")]
        public decimal MaterialCost { get; set; }
        [Display(Name = "Direct labour Cost")]
        public decimal LabourCost { get; set; }
        [Display(Name = "OverHead Cost(%)")]
        [Required]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Manuf. Cost")]
        public decimal ManufacturingCost { get; set; }
        [Display(Name = "Admin Cost(%)")]
        [Required]
        public decimal AdministrativeCost { get; set; }
        [Display(Name = "Total Cost")]
        public decimal GrandTotalCost { get; set; }
        //Fk
        public int EstimationSummaryId { get; set; }
        
    }

    public class EstimationCostSummary
    {
        [Key]
        public int EstimationCostSummaryId { get; set; }

        public decimal CommulativeCost { get; set; }
        [Display(Name = "Profit Margin(%)")]
        [Required]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Final Price")]
        public decimal FinalPriceIncludingProfit { get; set; }

        public bool IsApprovedMargin { get; set; }
        //Fk
        public int EstimationSummaryId { get; set; }

    }


}
