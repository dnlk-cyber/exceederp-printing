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
        public string JobNumber { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCostBirr { get; set; }
        public int UnitCostCent { get; set; }
        public decimal TotalPriceBirr { get; set; }
        public int TotalPriceCent { get; set; }
    }
}
