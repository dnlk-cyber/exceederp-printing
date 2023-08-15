using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class CommissionByPartnerAndAgentViewModel
    {
        public string TransactId { get; set; }
        public string PartnerType { get; set; }
        public string PartnerName { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public int SaleOrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string AccountCode { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public string OrderType { get; set; }
        public string SalesPerson { get; set; }
        public string TransactionRef { get; set; }
        public string InvoiceStatus { get; set; }
    }
}
