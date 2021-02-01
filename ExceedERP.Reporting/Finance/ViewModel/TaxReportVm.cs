using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class TaxReportVm
    {
        public string CustomerOrSupplier { get; set; }
        public string ServiceOrProduct { get; set; }
        public string Reference { get; set; }

        public DateTime EffectiveDate { get; set; }

        public decimal SalesOrPurchaseAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string TaxType { get; set; }
    }
}
