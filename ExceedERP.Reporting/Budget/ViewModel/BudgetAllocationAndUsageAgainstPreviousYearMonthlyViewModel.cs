using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System.Collections.Generic;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class BudgetAllocationAndUsageAgainstPreviousYearMonthlyViewModel
    {
        public string Month { get; set; }
        public List<BudgetAllocationAndUsageAgainstPreviousYearViewModel> List { get; set; }
    }
}
