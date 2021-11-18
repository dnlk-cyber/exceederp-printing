using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class ProformaItemVM
    {
        public int ProformaItemId { get; set; }
        public string Description { get; set; }
        public string SubCategory { get; set; }
        public decimal Quantity { get; set; }
        public string Page { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BeforeVat { get; set; }
        public string TaxPercent { get; set; }
        public decimal TotalPrice { get; set; }
        public string Observation { get; set; }
    }
}
