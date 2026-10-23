using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class BudgetAllocationAndUsageViewModel 
    {
        public int Index { get; set; }
        public string AccountCode { get; set; }
        public string BudgetCode { get; set; }
        public string Unit { get; set; }
        public string Month { get; set; }
        //[Display(Name = "Work Unit")]
        //public string WorkUnit { get; set; }
        //public decimal TotalYearBudget { get; set; }
        //public decimal YearlyActuals { get; set; }
        public decimal MonthlyBudget { get; set; }
        public decimal MonthlyActuals { get; set; }
        public decimal MonthlyVariance { get; set; }
        //public decimal YTD { get; set; }
    }
}
