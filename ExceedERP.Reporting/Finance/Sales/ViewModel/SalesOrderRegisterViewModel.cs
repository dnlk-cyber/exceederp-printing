using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   public class SalesOrderRegisterViewModel
    {
        public int SaleOrderNo { get; set; }
        public DateTime SaleOrderDate { get; set; }
        public string BroadCastDate { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
