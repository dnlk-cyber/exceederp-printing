using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
   public class MaterialConsumptionViewModel
    {
        public string Description { get; set; }
        public string Branch { get; set; }
        public string ItemName { get; set; }
        public decimal Amount { get; set; }
    }
}
