namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class QuarterlyPlanVM
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
        public decimal AnnualPlan { get; set; }
        public decimal Target { get; set; }
        public decimal QuarterOne { get; set; }
        public decimal QuarterTwo { get; set; }
        public decimal QuarterThree { get; set; }
        public decimal QuarterFour { get; set; }
    }
}
