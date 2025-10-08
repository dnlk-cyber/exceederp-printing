using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintEstimationMonthlyReportViewModel
    {
        public string CustomerName { get; set; }
       
        public List<PrintJobVm> PrintJobVms { get; set; }
    }
    public class PrintJobVm
    {
        public string JobType { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
    }
}
