using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.ViewModel
{
   public class SalesJournalViewModel
    {
        public string TransactId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string SaleOrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string AccountCode { get; set; }
        public string AcctDescription { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string TransactionRef { get; set; }
        public string PartnerType { get; set; }
        public string SalesPerson { get; set; }
        public string ItemDescription { get; set; }
    }
}
