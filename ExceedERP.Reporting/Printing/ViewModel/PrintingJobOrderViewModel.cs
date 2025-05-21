using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintingJobOrderViewModel : PrintingCommonViewModel
    {
        public string CustomerName { get; set; }
        public string CRVNo { get; set; }
        public DateTime JobOrderStartDate { get; set; }
        public DateTime JobDeliveryDate { get; set; }
        public string Remark { get; set; }

    }
    public class JobOrderDetail
    {
        public string JobOrderNo { get; set; } 
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public decimal Quantity { get; set; }
        public int Pages { get; set; }
        public string BindingStyle { get; set; }
        public string PaperSize { get; set; }
        public int TextColor { get; set; }
        public int CoverColor { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; } 
    }
}
