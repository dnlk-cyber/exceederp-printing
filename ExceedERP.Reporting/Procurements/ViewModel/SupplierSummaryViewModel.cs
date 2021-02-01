using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class SupplierSummaryViewModel
    {
        //public string Description { get; set; }
        //public string Unit { get; set; }
        //public decimal Quantity { get; set; }
        //public decimal UnitPrice { get; set; }
        //public decimal TotalPrice { get; set; }
        //public decimal SubTotal { get; set; }
        //public decimal Vat { get; set; }
        //public decimal GTotal{get;set;}
        public string WinnerSupplierName { get; set; }
        public List<WinningSupplierDetailVM> WinningSupplier { get; set; }
    }
}
