using ExceedERP.Helpers.Budget;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.Ratio.ViewModel
{
    public class RatioAnnualSummaryViewModel  
    {

        public string Type { get; set; }
        public string Name { get; set; }
        public decimal currentBudget { get; set; }
        public decimal firstQ1 { get; set; }
        public decimal firstQ2 { get; set; }
        public decimal firstQ3 { get; set; }
        public decimal firstQ4 { get; set; }

        public string firstIndustryAverage { get; set; }
        public string firstRemark { get; set; }



        public decimal secondQ1 { get; set; }
        public decimal secondQ2 { get; set; }
        public decimal secondQ3 { get; set; }

        public decimal secondQ4 { get; set; }
        public string secondIndustryAverage { get; set; }
        public string secondRemark { get; set; }

        public decimal thirdQ1 { get; set; }
        public decimal thirdQ2 { get; set; }
        public decimal thirdQ3 { get; set; }

        public decimal thirdQ4 { get; set; }
        public string thirdIndustryAverage { get; set; }
        public string thirdRemark { get; set; }

        public decimal fourthQ1 { get; set; }
        public decimal fourthQ2 { get; set; }
        public decimal fourthQ3 { get; set; }

        public decimal fourthQ4 { get; set; }

        public string fourthIndustryAverage { get; set; }
        public string fourthRemark { get; set; }
    }
}
