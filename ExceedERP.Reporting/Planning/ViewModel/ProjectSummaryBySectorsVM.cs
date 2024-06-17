using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class ProjectSummaryBySectorsVM
    {
        public string ProjectSector { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal ContratAmount { get; set; }
        public decimal Plan { get; set; }
        public decimal Excution { get; set; }
        public decimal Percent { get; set; }
        public decimal ToMonthPlan { get; set; }
        public decimal ToMonthExcution { get; set; }
        public decimal ToMonthPercent { get; set; }
        public decimal ExecuteSinceCommencement { get; set; }
        public decimal ContractPerformance { get; set; }
        public decimal WeeklyExpenses { get; set; }
        public decimal ProfitOrLossOfTheWeek { get; set; }
        public decimal ExpensesUpToThisWeek { get; set; }
        public decimal ProfitOrLossUpToWeek { get; set; }
        public decimal ActionPlanNextWeek { get; set; }
        public List<ProjectSummaryBySectorDetailVM> projectSummaryBySectorDetails { get; set; }
    }
    
    public class ProjectSummaryBySectorDetailVM
    {
        public string ProjectType { get; set; }
        public decimal AnnualPlan { get; set; }
        public decimal ContratAmount { get; set; }
        public decimal Plan { get; set; }
        public decimal Excution { get; set; }
        public decimal Percent { get; set; }
        public decimal ToMonthPlan { get; set; }
        public decimal ToMonthExcution { get; set; }
        public decimal ToMonthPercent { get; set; }
        public decimal ExecuteSinceCommencement { get; set; }
        public decimal ContractPerformance { get; set; }
        public decimal WeeklyExpenses { get; set; }
        public decimal ProfitOrLossOfTheWeek { get; set; }
        public decimal ExpensesUpToThisWeek { get; set; }
        public decimal ProfitOrLossUpToWeek { get; set; }
        public decimal ActionPlanNextWeek { get; set; }
    }
}
