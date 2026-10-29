namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class QuickFixPlanVM
    {
        public int SerialNumber { get; set; }
        public string FiscalYear { get; set; }
        public string MajorActivity { get; set; }
        public string Unit { get; set; }
        public decimal AnnualPlan { get; set; }
        public string UpToQuarter { get; set; }
        public decimal PlanUpToQuarter { get; set; }
        public decimal Actual { get; set; }
        public decimal PreviousPlanRemainingQuarter { get; set; }
        public decimal RevisedPlanRemainingQuarter { get; set; }
    }

    public class QuickFixQuarterVM
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
