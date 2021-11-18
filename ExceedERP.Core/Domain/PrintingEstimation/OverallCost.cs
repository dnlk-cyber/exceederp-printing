using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.PrintingEstimation
{
    public class OverallCost
    {
        public int OverallCostId { get; set; }
        public int EstimationFormId { get; set; }
        public EstimationForm EstimationForm { get; set; }
        [ReadOnly(true)]
        public decimal MaterialCost { get; set; }

        [ReadOnly(true)]
        public decimal LaborCost { get; set; }

        public decimal OverHeadCost { get; set; }
        [Required]
        [ReadOnly(true)]
        public decimal T { get; set; }
        public decimal MarginalCost { get; set; }
        [Required]
        [ReadOnly(true)]
        public decimal NBT { get; set; }
        public decimal TInpercent { get; set; }
        [Required]
        [ReadOnly(true)]
        public decimal GT { get; set; }
        public decimal Graphic { get; set; }
        [Required]
        [ReadOnly(true)]
        public decimal EstimatedNetPrice {get; set;}



    }
}
