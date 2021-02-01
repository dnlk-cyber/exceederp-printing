using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
  public class SupplierTransactionHistoryViewModel
    {
        public string SupplierName { get; set; }
        public int InvoiceNo { get; set; }
        public List<SupplierTransactionListsViewModel> SupplierTransactionLists { get; set; }
    }
}
