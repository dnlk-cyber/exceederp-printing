using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class PurchaseReportViewModel
    {
        public string ItemCategory { get; set; }
        public string RefNo { get; set; }
        public DateTime? Date { get; set; }
        public string PurchaseMethod { get; set; }
        public string CurrentStatus { get; set; }
        public string DurationForComplate { get; set; }
        public decimal BudgetToComplete { get; set; }
    }
}
