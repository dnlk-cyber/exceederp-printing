using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureProductDeliveryItems
    {
        public int FurnitureProductDeliveryItemsId { get; set; }
        public int FurnitureProductDeliveryId { get; set; }
        public int FurnitureJobId { get; set; }
        public string ItemCategory { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal VAT { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalCost { get; set; }
    }
    
}
