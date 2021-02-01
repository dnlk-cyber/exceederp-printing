using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
   public class CostsToGetMednInsuranceViewModel
    {
        public string Type { get; set; }
        public string ArbonCostPrevousYear { get; set; }
        public decimal ArbonCostCurrentYear { get; set; }
        public decimal Difference { get; set; }
    }
}
