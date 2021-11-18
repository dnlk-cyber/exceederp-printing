using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class ProductDeliveryItemsVM
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
