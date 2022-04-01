using ExceedERP.Core.Domain.PrintingEstimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.PrintingEstimation.ViewModel
{
    public class PrintingEstimationFormVieModel
    {
        public string Customer { get; set; }

        public DateTime Date { get; set; }
        public string JobNumber { get; set; }
        public decimal Quantity { get; set; }
        public string Size { get; set; }
        public string TextPaper { get; set; }
        public string TextPrint { get; set; }
        public string CoverPrint { get; set; }
        public string BindingStyle { get; set; }
        public string Others { get; set; }
      

        public virtual ICollection<MaterialCostViewModel> MaterialCosts { get; set; } = new List<MaterialCostViewModel>();
        public virtual ICollection<LaborCostViewModel> LaborCosts { get; set; } = new List<LaborCostViewModel>();
        public virtual OverallCostViewModel OverallCostViewModel { get; set; }

    }
    public class MaterialCostViewModel
    {
        public string Material { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class LaborCostViewModel
    {
        public PrintingSteps PrintingSteps { get; set; }
        public string PrintingStepvalue { get; set; }
        public decimal WorkingTime { get; set; }

        public decimal CostPerHour { get; set; }

        public decimal TotalCost { get; set; }
    }
    public class OverallCostViewModel
    {
        public decimal MaterialCost { get; set; }

        public decimal LaborCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal T { get; set; }
        public decimal MarginalCost { get; set; }
        public decimal NBT { get; set; }
        public decimal TInpercent { get; set; }
        public decimal GT { get; set; }
        public decimal Graphic { get; set; }
        public decimal EstimatedNetPrice { get; set; }
        public virtual PrintingEstimationFormVieModel PrintinEstimationFormVieModel { get; set; }

    }
}
