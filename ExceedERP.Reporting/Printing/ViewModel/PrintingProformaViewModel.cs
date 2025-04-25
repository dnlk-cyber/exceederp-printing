using ExceedERP.Core.Domain.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintingProformaViewModel
    {
        public int ProformaNumber { get; set; }
        public string CustomerName { get; set; }
        public int DeliveryPeriod { get; set; }
        public string Remark { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
    public class PrintingProformaItemsViewModel
    {

        public PrintJobType JobType { get; set; }
        public string JobDescription { get; set; }
        public decimal Quantity { get; set; }
        public int NoOfPage { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string VAT { get; set; }
        public string TransportVAT { get; set; }
        public decimal TransportCost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingPriceIncludingTransCost { get; set; }
        public string Remark { get; set; } 
    }
}
