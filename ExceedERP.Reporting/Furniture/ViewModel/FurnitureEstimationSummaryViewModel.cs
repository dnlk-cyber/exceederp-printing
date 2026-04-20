using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureEstimationSummaryViewModel : FurnitureCommonViewModel
    {
        public string CustomerName { get; set; }
        public string Status { get; set; } 
    }
    public class FurnitureSummaryDetail
    {
        public string JobType { get; set; }
        public decimal Quantity { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ManufacturingCost { get; set; }
        public decimal AdminCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ProfitMargin { get; set; }



    }
}
