using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
   public class DrugSaleDetailViewModel
    {
        public string DrugCode { get; set; }
        public string Description { get; set; }
        public decimal TotalQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Unit { get; set; }
    }
}
