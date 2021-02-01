using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class TaxRateVm
    {
        public int GLTaxRateID { get; set; }
      
        [Display(Name = "Tax Name")]
        public string TaxName { get; set; }
    }
}
