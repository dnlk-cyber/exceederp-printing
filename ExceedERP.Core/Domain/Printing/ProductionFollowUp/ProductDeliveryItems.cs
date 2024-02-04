using ExceedERP.Core.Domain.printing.JobCosting.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
   public class ProductDeliveryItems
    {
        public int ProductDeliveryItemsId { get; set; }
        public int ProductDeliveryId { get; set; }
        public int JobId { get; set; }
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
