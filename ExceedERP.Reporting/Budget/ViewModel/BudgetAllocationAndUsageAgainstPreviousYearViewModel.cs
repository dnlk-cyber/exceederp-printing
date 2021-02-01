using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class BudgetAllocationAndUsageAgainstPreviousYearViewModel : Organization
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Budget Category")]
        public string BudgetCategory { get; set; }

        [Display(Name = " Fiscal Year")]
        public string FiscalYear { get; set; }
       
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Work Unit")]
        public string WorkUnit { get; set; }
        public string Unit { get; set; }
        public decimal ThisYearBudget { get; set; }
        public decimal PreviousYearBudget { get; set; }
        public decimal BudgetDifference { get; set; }
        public decimal BudgetDifferencePercentage { get; set; }
        public decimal ThisYearActual { get; set; }
        public decimal PreviousYearActual { get; set; }
        public decimal ActualDifference { get; set; }
        public decimal ActualDifferencePercentage { get; set; }
    }
}
