using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.PrintingEstimation.ViewModel
{
    public class ExecutiveSummaryVM
    {
        public string KeyPerformance { get; set; }
        public string Unit { get; set; }
        public decimal ThisWeekPlan { get; set; }
        public decimal ThisWeekActual { get; set; }
        public decimal ThisWeekPercent{ get; set; }
        public decimal UpToThisWeekPlan { get; set; }
        public decimal UPToThisWeekActual { get; set; }
        public decimal UPToThisWeekPercent { get; set; }
    }
}
