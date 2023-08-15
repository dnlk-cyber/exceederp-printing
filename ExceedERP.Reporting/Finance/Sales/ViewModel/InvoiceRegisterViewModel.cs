using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class InvoiceRegisterViewModel
    {
        public string CustomoerName { get; set; }
        public int SaleOrderNo { get; set; }
        public int InvoiceNo { get; set; }
        public string Reference { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
    }
}
