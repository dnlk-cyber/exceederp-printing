namespace ExceedERP.Reporting.Planning.ViewModel
{
    public class StrategicPlanReportVM
    {
        public int SerialNumber { get; set; }
        public string PlanPeriod { get; set; }
        public string KeyResultArea { get; set; }
        public string ItemType { get; set; }
        public string FiscalYear { get; set; }
        public string Name { get; set; }
        public string WorkUnit { get; set; }
        public decimal? Weight { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    public class StrategicPlanLookupVM
    {
        public int StrategicPlanId { get; set; }
        public string DisplayName { get; set; }
    }
}
