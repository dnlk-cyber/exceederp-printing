using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;

namespace ExceedERP.Reporting.Finance.ViewModel
{
   public class ProductSoldToCustomerVm
    {
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string PartnerName { get; set; }
        public OrderType OrderType { get; set; }
        public int OrderNo { get; set; }
        public string OrderReference { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesTax { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
