using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class BudgetAllocationAndUsageViewModel : Organization
    {
        public string Index { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        public string Unit { get; set; }
        public string Month { get; set; }
        [Display(Name = "Work Unit")]
        public string WorkUnit { get; set; }
        public decimal TotalYearBudget { get; set; }
        public decimal YearlyActuals { get; set; }
        public decimal MonthlyBudget { get; set; }
        public decimal MonthlyActuals { get; set; }
        public decimal MonthlyVariance { get; set; }
        public decimal YTD { get; set; }
    }
}
