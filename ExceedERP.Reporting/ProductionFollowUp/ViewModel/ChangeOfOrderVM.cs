using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class ChangeOfOrderVM
    {
        public string JobOrderNo { get; set; }
        public string Reason { get; set; }
        public double Quantity { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BeforeTax { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public string InvoiceNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PreparedBy { get; set; }
        public string TypeAndQuantityOfMaterialNeeded { get; set; }
        public string Checked { get; set; }
    }
}
