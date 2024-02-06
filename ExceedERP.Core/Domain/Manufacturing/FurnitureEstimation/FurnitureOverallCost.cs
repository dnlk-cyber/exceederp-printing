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
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal OverHeadCost { get; set; }
        [Required]
        public decimal T { get; set; }
        [Display(Name = "Marginal Cost")]
        public decimal MarginalCost { get; set; }
        [Required]
        public decimal NBT { get; set; }
        [Display(Name = "Tin Percent")]
        public decimal TInpercent { get; set; }
        [Required]
        public decimal GT { get; set; }
        public decimal Graphic { get; set; }
        [Required]
        public decimal EstimatedNetPrice { get; set; }
        public int FurnitureEstimationId { get; set; }
        public FurnitureEstimationForm Estimation { get; set; }
    }
}
