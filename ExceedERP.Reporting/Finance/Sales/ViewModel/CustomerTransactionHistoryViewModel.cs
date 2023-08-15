using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class CustomerTransactionHistoryViewModel
    {
        public string CustomerName { get; set; }
        public int InvoiceNo { get; set; }
        public List<CustomerTransactionLists> CustomerTransactionLists { get; set; }
       
    }
}
