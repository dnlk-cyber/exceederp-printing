using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Core.Reporting.ViewModel
{
    public class WorkUnitReportViewModel : Organization
    {
   
        public string Unit { get; set; }
        public string WorkUnit { get; set; }
        public decimal ThisYearBudget { get; set; }
        public decimal ThisYearMonthlyPlan  { get; set; }
        public decimal ThisYearMonthlyActual  { get; set; }
         public decimal MonthlyDiffrencePecent { get; set; }
        public decimal CurrentYearPlan { get; set; }
        public decimal CurrentYearActual { get; set; }
        public decimal CurrentYearDifference { get; set; }
        public decimal GrandDifferencePercentage { get; set; }
   
    }
}
