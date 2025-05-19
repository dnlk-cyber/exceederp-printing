
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
        public decimal TotalProductionCost { get; set; } // 
        [Display(Name = "Admin Cost")]
        public decimal AdminstrativeCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal AdminRate { get; set; }
        public decimal OverHeadRate { get; set; }
        public decimal IndirectMaterialCost { get; set; } // 3% Variable Cost
        public decimal MaterialHandlingStorageWaste { get; set; } // 2.5% TotalProductionCost
        [Display(Name = "Profit Margin")]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Graphics Cost")]
        public decimal GraphicsCost { get; set; }
        public decimal GraphicsRate { get; set; }
       
        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; } // TotalProductionCost + Material Handling and storage waste
        public decimal SellingPrice { get; set; }
        
        public int PrintingCostEstimationId { get; set; } 

    }
}
