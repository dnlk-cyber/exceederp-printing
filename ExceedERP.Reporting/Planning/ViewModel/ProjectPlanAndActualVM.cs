using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class ProjectPlanAndActualVM
    { 

        public string FiscalYear { get; set; }
        public string Project { get; set; }
        public string Objective { get; set; }
        public string KPI { get; set; }
        public string SubKPIId { get; set; }
        public string Description { get; set; }
        public List<ProjectPlanActualItem> ActualItems { get; set; } = new List<ProjectPlanActualItem>();
        public List<ProjectActualVsPlanMontlyItems> ActualVsPlanItems { get; set; }




        public class ProjectPlanActualItem
        {

            public decimal July { get; set; }
            public decimal JulyPlan { get; set; }
            public decimal JulyPercent { get; set; }
            public decimal Augest { get; set; }
            public decimal AugestPlan { get; set; }
            public decimal AugestPercent { get; set; }
            public decimal September { get; set; }
            public decimal SeptemberPlan { get; set; }
            public decimal SeptemberPercent { get; set; }
            public decimal October { get; set; }
            public decimal OctoberPlan { get; set; }
            public decimal OctoberPercent { get; set; }
            public decimal November { get; set; }
            public decimal NovemberPlan { get; set; }
            public decimal NovemberPercent { get; set; }
            public decimal December { get; set; }
            public decimal DecemberPlan { get; set; }
            public decimal DecemberPercent { get; set; }
            public decimal January { get; set; }
            public decimal JanuaryPlan { get; set; }
            public decimal JanuaryPercent { get; set; }
            public decimal February { get; set; }
            public decimal FebruaryPlan { get; set; }
            public decimal FebruaryPercent { get; set; }
            public decimal March { get; set; }
            public decimal MarchPlan { get; set; }
            public decimal MarchPercent { get; set; }
            public decimal April { get; set; }
            public decimal AprilPlan { get; set; }
            public decimal AprilPercent { get; set; }
            public decimal May { get; set; }
            public decimal MayPlan { get; set; }
            public decimal MayPercent { get; set; }
            public decimal June { get; set; }
            public decimal JunePlan { get; set; }
            public decimal JunePercent { get; set; }
            public decimal ToDate { get; set; }
            public decimal ToDatePlan { get; set; }
            public decimal ToDateActual { get; set; }
            public decimal Yearly { get; set; }
            public decimal YearlyPlan { get; set; }
            public decimal YearlyActual { get; set; }
            public decimal JulyWeek1 { get; set; }
            public decimal JulyWeek2 { get; set; }
            public decimal JulyWeek3 { get; set; }
            public decimal JulyWeek4 { get; set; }
            public decimal AugestWeek1 { get; set; }
            public decimal AugestWeek2 { get; set; }
            public decimal AugestWeek3 { get; set; }
            public decimal AugestWeek4 { get; set; }
            public decimal SeptemberWeek1 { get; set; }
            public decimal SeptemberWeek2 { get; set; }
            public decimal SeptemberWeek3 { get; set; }
            public decimal SeptemberWeek4 { get; set; }
            public decimal OctoberWeek1 { get; set; }
            public decimal OctoberWeek2 { get; set; }
            public decimal OctoberWeek3 { get; set; }
            public decimal OctoberWeek4 { get; set; }
            public decimal NovemberWeek1 { get; set; }
            public decimal NovemberWeek2 { get; set; }
            public decimal NovemberWeek3 { get; set; }
            public decimal NovemberWeek4 { get; set; }
            public decimal DecemberWeek1 { get; set; }
            public decimal DecemberWeek2 { get; set; }
            public decimal DecemberWeek3 { get; set; }
            public decimal DecemberWeek4 { get; set; }
            public decimal JanuaryWeek1 { get; set; }
            public decimal JanuaryWeek2 { get; set; }
            public decimal JanuaryWeek3 { get; set; }
            public decimal JanuaryWeek4 { get; set; }
            public decimal FebruaryWeek1 { get; set; }
            public decimal FebruaryWeek2 { get; set; }
            public decimal FebruaryWeek3 { get; set; }
            public decimal FebruaryWeek4 { get; set; }
            public decimal MarchWeek1 { get; set; }
            public decimal MarchWeek2 { get; set; }
            public decimal MarchWeek3 { get; set; }
            public decimal MarchWeek4 { get; set; }
            public decimal AprilWeek1 { get; set; }
            public decimal AprilWeek2 { get; set; }
            public decimal AprilWeek3 { get; set; }
            public decimal AprilWeek4 { get; set; }
            public decimal MayWeek1 { get; set; }
            public decimal MayWeek2 { get; set; }
            public decimal MayWeek3 { get; set; }
            public decimal MayWeek4 { get; set; }
            public decimal JuneWeek1 { get; set; }
            public decimal JuneWeek2 { get; set; }
            public decimal JuneWeek3 { get; set; }
            public decimal JuneWeek4 { get; set; }
        }
        public class ProjectActualVsPlanMontlyItems
        {

          
            public decimal MonthActual { get; set; }
            public decimal MonthPlan { get; set; }
            public decimal MonthPercent { get; set; }
            public decimal ToDate { get; set; }
            public decimal ToDatePlan { get; set; }
            public decimal ToDateActual { get; set; }
            public decimal Yearly { get; set; }
            public decimal YearlyPlan { get; set; }
            public decimal YearlyActual { get; set; }

            public List<ProjectActualVsPlanWeeklItems> WeeklyActualVsPlanItems { get; set; }



        }
        public class ProjectActualVsPlanWeeklItems
        {

            public decimal ActualWeek1 { get; set; }
            public decimal ActualWeek2 { get; set; }
            public decimal ActualWeek3 { get; set; }
            public decimal ActualWeek4 { get; set; }
            public decimal PlanWeek1 { get; set; }
            public decimal PlanWeek2 { get; set; }
            public decimal PlanWeek3 { get; set; }
            public decimal PlanWeek4 { get; set; }
            public decimal PercentWeek1 { get; set; }
            public decimal PercentWeek2 { get; set; }
            public decimal PercentWeek3 { get; set; }
            public decimal PercentWeek4 { get; set; }
        }
    }
}
