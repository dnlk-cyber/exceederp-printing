using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting
{
   public class JobOrderSummary
    {
        public int JobOrderSummaryId { get; set; }
        public string JobId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public int Page { get; set; }
        public double SellingPrice { get; set; }
        public double DirectMaterial { get; set; }
        public double DirectLabor { get; set; }
        public double ManufacturingOverHead { get; set; }
        public double SellingAndDistribution { get; set; }
        public double AdminAndGeneralExpense { get; set; }
        public double TotalCost { get; set; }
        public double ProfitOrLoss { get; set; }
        public int Percentage { get; set; }
    }
}
