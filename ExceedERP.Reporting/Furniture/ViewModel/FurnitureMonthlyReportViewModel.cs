using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureMonthlyReportViewModel
    {
        public string CustomerName { get; set; }
       
        public List<FurnitureJobVm> FurnitureJobVms { get; set; }
        public decimal TotalQuantity => FurnitureJobVms?.Sum(s => s.Quantity) ?? 0;
        public decimal TotalCost => FurnitureJobVms?.Sum(s => s.TotalCost) ?? 0;
    }
    public class FurnitureJobVm
    {
        public string JobType { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
    }
}
