using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class JobOrderMaterialVM
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string UoM { get; set; }
        public double Gram { get; set; }
        public double Size { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BeforeTax { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public string SIVNo { get; set; }
        public string Remark { get; set; }
    }
}
