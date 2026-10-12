namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class MonthlyStaffMovementHeaderInfoVM
    {
        public string FiscalYearName { get; set; }
        // Selected GLPeriod names joined with "+", e.g. "ሐምሌ+ነሐሴ+መስከረም"; falls back to a formatted date range when no period is selected
        public string PeriodLabel { get; set; }
        // Monthly/Quarterly/Semi-Annual/Annual title, derived from the selected period count or date range span
        public string ReportTitle { get; set; }
    }
}
