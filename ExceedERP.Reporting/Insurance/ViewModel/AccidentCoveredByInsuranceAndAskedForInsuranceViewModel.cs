using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
    public class AccidentCoveredByInsuranceAndAskedForInsuranceViewModel
    {
        public string InsuranceTypePeriviousYear { get; set; }
        public string InsuranceTypeCurrentYear { get; set; }
        public int? PaymentPreviousYear { get; set; }
        public int? PaymentCurrentYear { get; set; }
        public string Difference { get; set; }
    }
}
