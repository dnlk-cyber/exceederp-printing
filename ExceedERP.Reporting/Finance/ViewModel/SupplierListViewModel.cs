using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class SupplierListViewModel
    {
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier ID")]
        public string ID { get; set; }
        public string Account { get; set; }
        public string TradeLicense { get; set; }
        public string IsCredit { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        public string IsActive { get; set; }
    }
}
