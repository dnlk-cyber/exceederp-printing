namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class PlanVsActualQuarterlyVM
    {
        public int SerialNumber { get; set; }
        public string WorkUnit { get; set; }
        public string Objective { get; set; }
        public string Indicator { get; set; }
        public string SubIndicator { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string KPIType { get; set; }
        public decimal Baseline { get; set; }
        public decimal Target { get; set; }

        public decimal AnnualPlan { get; set; }
        public decimal AnnualActual { get; set; }
        public decimal AnnualPercent { get; set; }

        public decimal QuarterOnePlan { get; set; }
        public decimal QuarterOneActual { get; set; }
        public decimal QuarterOnePercent { get; set; }
        public decimal QuarterTwoPlan { get; set; }
        public decimal QuarterTwoActual { get; set; }
        public decimal QuarterTwoPercent { get; set; }
        public decimal QuarterThreePlan { get; set; }
        public decimal QuarterThreeActual { get; set; }
        public decimal QuarterThreePercent { get; set; }
        public decimal QuarterFourPlan { get; set; }
        public decimal QuarterFourActual { get; set; }
        public decimal QuarterFourPercent { get; set; }
    }
}
