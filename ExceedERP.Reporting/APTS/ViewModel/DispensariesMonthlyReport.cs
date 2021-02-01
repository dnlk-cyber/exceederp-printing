using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
   public class DispensariesMonthlyReport
    {
        public decimal TotalCostOfBegginingStock { get; set; }
        public decimal TotalCostOfReceivedStock { get; set; }
        public decimal CostOfStockAvailableForSale { get; set; }
    }
}
