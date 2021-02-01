using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class CustomerTransactionLists
    {
      
        public string Transaction { get; set; }
        public string InvoiceNo { get; set; }
        public int TransNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
