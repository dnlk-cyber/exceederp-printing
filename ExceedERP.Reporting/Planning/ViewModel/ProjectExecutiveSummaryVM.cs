using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class ProjectExecutiveSummaryVM
    {
        public string MajorActivities { get; set; }
        public string Unit { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal ContratAmount { get; set; }
        public decimal Plan { get; set; }
        public decimal Actual { get; set; }
        public decimal Percent { get; set; }
        public decimal ToMonthPlan { get; set; }
        public decimal ToMonthActual { get; set; }
        public decimal ToMonthPercent { get; set; }
        public List<ProjectExecutiveSummaryDetailVM> ProjectExecutives { get; set; }
    }
    public class ProjectExecutiveSummaryDetailVM
    {
        public string MajorActivities { get; set; }
        public string Unit { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal ContratAmount { get; set; }
        public decimal Plan { get; set; }
        public decimal Actual { get; set; }
        public decimal Percent { get; set; }
        public decimal ToMonthPlan { get; set; }
        public decimal ToMonthActual { get; set; }
        public decimal ToMonthPercent { get; set; }
        public List<ProjectInformationExecutiveSummaryDetailVM> projectInformation { get; set; }
    }
    public class ProjectInformationExecutiveSummaryDetailVM
    {
        public string Project { get; set; }
        public string Unit { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal ContratAmount { get; set; }
        public decimal Plan { get; set; }
        public decimal Actual { get; set; }
        public decimal Percent { get; set; }
        public decimal ToMonthPlan { get; set; }
        public decimal ToMonthActual { get; set; }
        public decimal ToMonthPercent { get; set; }
    }
}
