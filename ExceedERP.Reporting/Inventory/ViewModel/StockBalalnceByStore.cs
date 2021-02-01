using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
   public class StockBalalnceByStoreVm
    {
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public string Branch { get; set; }
        public decimal Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Totalamount { get; set; }
    }
}
