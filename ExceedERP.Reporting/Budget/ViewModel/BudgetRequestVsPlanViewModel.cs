using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.ViewModel
{
   public class BudgetRequestVsPlanViewModel
    {
        public string Year { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string AccountCode { get; set; }
        public decimal Actual { get; set; }
        public decimal Utilaization { get; set; }
        public decimal Plan { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercent { get; set; }
    }
}
