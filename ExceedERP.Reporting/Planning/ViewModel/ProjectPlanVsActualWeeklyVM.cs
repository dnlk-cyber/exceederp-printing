using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class ProjectPlanVsActualWeeklyVM
    {

        public string FiscalYear { get; set; }
        public List<ProjectPlanVsActualKPIVM> ActualItems { get; set; }
       

    }
    public class ProjectPlanVsActualKPIVM
    {

        public string Project { get; set; }
        public string Objective { get; set; }
        public string KPI { get; set; }
        public string SubKPIId { get; set; }
        public string Description { get; set; }
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

        public List<ProjectActualVsPlanMontlyKPIItems> ProjectActualVsPlanMOnthlyItems { get; set; }

    }


    public class ProjectActualVsPlanMontlyKPIItems
    {

          
        public string Month { get; set; }
        public decimal MonthActual { get; set; }
        public decimal MonthPlan { get; set; }
        public decimal MonthPercent { get; set; }
        public decimal ToDate { get; set; }
        public decimal ToDatePlan { get; set; }
        public decimal ToDateActual { get; set; }
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

        public List<ProjectActualVsPlanWeeklyKPIItems> ProjectWeeklyActualVsPlanItems { get; set; }



        }

    public class ProjectActualVsPlanWeeklyKPIItems
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

