using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
    public class DepreciationHeaderViewModel : Organization
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }
    public class DepreciationViewModel
    {
        public string Group { get; set; }
        public string Category { get; set; }
        public string Branch { get; set; }
        public decimal DepreciationAmount { get; set; }

    }
   
}
