using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.Ratio.ViewModel
{
    public class RatioAnalysisViewModel : Organization
    {

        public string Type { get; set; }
        public string RatioName { get; set; }
        public decimal Actual { get; set; }
        public decimal Budget { get; set; }
        public string IndustryAverage { get; set; }
        public string Remark { get; set; }
    }
}
