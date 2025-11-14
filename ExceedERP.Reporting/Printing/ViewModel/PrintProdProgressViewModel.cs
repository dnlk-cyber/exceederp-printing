using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintProdProgressViewModel
    {
        public string JobType { get; set; }
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }
        public string TextPaper { get; set; }
        public int Page { get; set; }
        public string JobDesc { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
