using ExceedERP.Core.Domain.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintingEstimationViewModel : PrintingCommonViewModel
    {
        public PrintJobType JobType { get; set; }
        public string CustomerName { get; set; }
        
        public string BindingStyle { get; set; }
        public int Quantity { get; set; }
        public int TextNoOfColors { get; set; }
        public int CoverNoOfColors { get; set; }
        public int NoOfPages { get; set; }
        public string JobDescription { get; set; }
        public string TextPaperSize { get; set; }
        public string CoverPaperSize { get; set; }
        public int EstimationNo { get; set; }
        public string MachineForText { get; set; }
        public string MachineForCover { get; set; }

    }

    public class PrintingMaterialViewModel
    {
        public EstimationMaterialType EstimationMaterialType { get; set; }

        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string UoM { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }

    }

    public class PrintingLaborViewModel
    {
        public PressCategory? PressCategory { get; set; }
        public LabourActivity? LaborActivity { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        
        public decimal LaborRate { get; set; }
        public decimal TotalCost { get; set; }

    }
    public class OverAllCostViewModel
    {
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal TotalProductionCost { get; set; }
        public decimal AdminCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal GraphicsCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal GrandTotal { get; set; } 
    }
}
