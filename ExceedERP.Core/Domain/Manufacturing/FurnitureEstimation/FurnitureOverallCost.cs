using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureOverallCost
    {
        [Key]
        public int FurnitureOverallCostId { get; set; }
        [Display(Name ="Material Cost")]
        public decimal MaterialCost { get; set; }
        [Display(Name ="Direct labour Cost")]
        public decimal LabourCost { get; set; }
        [Display(Name ="OverHead Cost(%)")]
        [Required]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Manuf. Cost")]
        public decimal ManufacturingCost { get; set; }
        [Display(Name = "Admin Cost(%)")]
        [Required]
        public decimal AdministrativeCost { get; set; }
        [Display(Name ="Total Cost")]
        public decimal GrandTotalCost { get; set; }
        [Display(Name = "Profit Margin(%)")]
        [Required]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Final Price")]
        public decimal FinalPriceIncludingProfit { get; set; }
        public int FurnitureEstimationId { get; set; }
        public FurnitureEstimationForm Estimation { get; set; }
    }
}
