using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureWeeklyReportViewModel
    {
        public string JobNo { get; set; }
        public string JobType { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProducedQuanttity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
