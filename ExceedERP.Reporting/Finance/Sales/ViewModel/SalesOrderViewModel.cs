using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class SalesOrderViewModel
    {
        public int SaleOrderNo { get; set; }
        public string SaleOrderStatus { get; set; }
        public DateTime SaleOrderDate { get; set; }
        public string BroadCastDate { get; set; }
        public string BroadCastStatus { get; set; }
        public string PaymentSchedule { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string ItemDescription { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal QtyInvoiced { get; set; }
        public decimal QtyRemaining { get; set; }
        public decimal AmountOrdered { get; set; }
        public decimal AmountInvoiced { get; set; }
        public decimal AmountRemaining { get; set; }
    }
}
