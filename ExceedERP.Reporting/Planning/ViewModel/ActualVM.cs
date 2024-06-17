using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class ActualVM : Organization
    {
        
        public string FiscalYear { get; set; }
        public string Description { get; set; }
        public List<ActualObjectiveItem> ObjectiveItems { get; set; } = new List<ActualObjectiveItem>();
        public class ActualObjectiveItem
        {
            public string Objective { get; set; }
            public string WorkUnit { get; set; }
            public string Description { get; set; }
            public List<IndicatorActualItems> IndicatorItems { get; set; } = new List<IndicatorActualItems>();

        }
        public class IndicatorActualItems
        {
            public string Indicators { get; set; }
            public List<ActualItem> ActualItems { get; set; } = new List<ActualItem>();

        }
        public class ActualItem
        {
            public string Indicators { get; set; }
            public string SubIndicators { get; set; }
            public string Unit { get; set; }
            public string KPIType { get; set; }
            public string Description { get; set; }
            public decimal AnnualPlan { get; set; }
            public decimal Baseline { get; set; }
            public decimal Target { get; set; }
            public decimal July { get; set; }
            public decimal JulyWeek1 { get; set; }
            public decimal JulyWeek2 { get; set; }
            public decimal JulyWeek3 { get; set; }
            public decimal JulyWeek4 { get; set; }
            public decimal Augest { get; set; }
            public decimal AugestWeek1 { get; set; }
            public decimal AugestWeek2 { get; set; }
            public decimal AugestWeek3 { get; set; }
            public decimal AugestWeek4 { get; set; }
            public decimal September { get; set; }
            public decimal SeptemberWeek1 { get; set; }
            public decimal SeptemberWeek2 { get; set; }
            public decimal SeptemberWeek3 { get; set; }
            public decimal SeptemberWeek4 { get; set; }
            public decimal October { get; set; }
            public decimal OctoberWeek1 { get; set; }
            public decimal OctoberWeek2 { get; set; }
            public decimal OctoberWeek3 { get; set; }
            public decimal OctoberWeek4 { get; set; }
            public decimal November { get; set; }
            public decimal NovemberWeek1 { get; set; }
            public decimal NovemberWeek2 { get; set; }
            public decimal NovemberWeek3 { get; set; }
            public decimal NovemberWeek4 { get; set; }
            public decimal December { get; set; }
            public decimal DecemberWeek1 { get; set; }
            public decimal DecemberWeek2 { get; set; }
            public decimal DecemberWeek3 { get; set; }
            public decimal DecemberWeek4 { get; set; }
            public decimal January { get; set; }
            public decimal JanuaryWeek1 { get; set; }
            public decimal JanuaryWeek2 { get; set; }
            public decimal JanuaryWeek3 { get; set; }
            public decimal JanuaryWeek4 { get; set; }
            public decimal February { get; set; }
            public decimal FebruaryWeek1 { get; set; }
            public decimal FebruaryWeek2 { get; set; }
            public decimal FebruaryWeek3 { get; set; }
            public decimal FebruaryWeek4 { get; set; }
            public decimal March { get; set; }
            public decimal MarchWeek1 { get; set; }
            public decimal MarchWeek2 { get; set; }
            public decimal MarchWeek3 { get; set; }
            public decimal MarchWeek4 { get; set; }
            public decimal April { get; set; }
            public decimal AprilWeek1 { get; set; }
            public decimal AprilWeek2 { get; set; }
            public decimal AprilWeek3 { get; set; }
            public decimal AprilWeek4 { get; set; }
            public decimal May { get; set; }
            public decimal MayWeek1 { get; set; }
            public decimal MayWeek2 { get; set; }
            public decimal MayWeek3 { get; set; }
            public decimal MayWeek4 { get; set; }
            public decimal June { get; set; }
            public decimal JuneWeek1 { get; set; }
            public decimal JuneWeek2 { get; set; }
            public decimal JuneWeek3 { get; set; }
            public decimal JuneWeek4 { get; set; }
        }
    }
}
