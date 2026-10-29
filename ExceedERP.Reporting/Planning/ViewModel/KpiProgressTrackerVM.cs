using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class KpiProgressTrackerVM
    {

        public string Index { get; set; }
        public string KPI { get; set; }
        public decimal? Weight { get; set; }
        public string WeightDisplay { get; set; }
        public decimal? AnnualPlan { get; set; }
        public decimal? Plan { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Percent { get; set; }
        public decimal? Evaluation { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public List<SubKpiProgressTrackerVM> SubKpiProgresses { get; set; }
    }

    public class SubKpiProgressTrackerVM
    {
        public string Index { get; set; }
        public string SubKPI { get; set; }
        public string Indicators { get; set; }
        public string Saf { get; set; }
        public decimal? Weight { get; set; }
        public string WeightDisplay { get; set; }
        public decimal? AnnualPlan { get; set; }
        public decimal? Plan { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Percent { get; set; }
        public decimal? Evaluation { get; set; }
        public string Remark { get; set; }
        public List<IndicatorsProgressTrackerVM> IndicatorsProgresses { get; set; } = new List<IndicatorsProgressTrackerVM>();


    }
    public class IndicatorsProgressTrackerVM
    {
        public string Indicators { get; set; }
        public string Saf { get; set; }
        public decimal? Weight { get; set; }
        public string WeightDisplay { get; set; }
        public decimal? AnnualPlan { get; set; }
        public decimal? Plan { get; set; }
        public decimal? Performance { get; set; }
        public decimal? Percent { get; set; }
        public decimal? Evaluation { get; set; }
        public string Remark { get; set; }
          
       
    }
}
