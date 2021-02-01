using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;

namespace ExceedERP.Reporting.Finance.ViewModel
{
   public class SaleStatusVm
    {

        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Reference { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatus { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoiceBalance { get; set; }
        public decimal RemainingQty { get; set; }
    }
}
