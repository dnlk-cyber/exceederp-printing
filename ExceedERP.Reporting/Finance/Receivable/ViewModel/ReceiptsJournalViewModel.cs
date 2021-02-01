using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.ViewModel
{
   public class ReceiptsJournalViewModel
    {
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string TransactionNo { get; set; }
        public string OrderNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
