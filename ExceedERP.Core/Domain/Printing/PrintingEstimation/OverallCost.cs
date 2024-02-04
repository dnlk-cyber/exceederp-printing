using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.PrintingEstimation
{
    public class OverallCost
    {
        public int OverallCostId { get; set; }
        public int EstimationFormId { get; set; }
        public EstimationForm EstimationForm { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }

        public decimal OverHeadCost { get; set; }
        [Required]
        public decimal T { get; set; }
        public decimal MarginalCost { get; set; }
        [Required]
        public decimal NBT { get; set; }
        public decimal TInpercent { get; set; }
        [Required]
        public decimal GT { get; set; }
        public decimal Graphic { get; set; }
        [Required]
        public decimal EstimatedNetPrice {get; set;}



    }
}
