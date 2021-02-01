using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel : Organization
    {
      
        public string AccountCode { get; set; }
       
        public string AccountDesc { get; set; }
        public string BudgetMonth { get; set; }
        public string Unit { get; set; }
        public string WorkUnit { get; set; }
        public decimal ThisYearBudget { get; set; }
        public decimal ThisYearMonthlyBudget { get; set; }
        public decimal MonthlyDiffrence { get; set; }
        public decimal MonthlyDiffrencePecent { get; set; }
        public decimal PreviousMonthlyBudget { get; set; }
        public decimal PreviousYearBudget { get; set; }
        public decimal BudgetDifference { get; set; }
        public decimal BudgetDifferencePercentage { get; set; }
        public decimal ThisYearActual { get; set; }
        public decimal PreviousYearActual { get; set; }
        public decimal ActualDifference { get; set; }
        public decimal ActualDifferencePercentage { get; set; }
    }
}
