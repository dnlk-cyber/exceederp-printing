using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class SupplierListViewModel
    {
        public string SupplierName { get; set; }
        public string ID { get; set; }
        public string Account { get; set; }
        public string TradeLicense { get; set; }
        public string Address { get; set; }
        public string IsCredit { get; set; }
        public string IsActive { get; set; }
    }
}
