
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingOverAllCost
    {
        [Key]
        public int PrintingOverAllCostId { get; set; }

        [Display(Name ="Material cost")]
        public decimal MaterialCost { get; set; }
        [Display(Name = "Labor Cost")]
        public decimal LaborCost { get; set; }
        [Display(Name = "Total Production Cost")]
        public decimal TotalProductionCost { get; set; }
        [Display(Name = "Admin Cost")]
        public decimal AdminstrativeCost { get; set; }
        [Display(Name = "Profit Margin")]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Graphics Cost")]
        public decimal GraphicsCost { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total Before VAT")]
        public decimal TotalBeforeVAT { get; set; }
        public decimal VAT { get; set; }
        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }

        public int PrintingCostEstimationId { get; set; } 

    }
}
