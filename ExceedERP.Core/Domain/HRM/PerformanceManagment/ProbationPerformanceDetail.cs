namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class ProbationPerformanceDetail
    {
        public int ProbationPerformanceDetailId { get; set; }
        public int ProbationPerformanceId { get; set; }
        public int Score { get; set; }
        public int LookupId { get; set; }
        public ProbationMeasurement Measurement { get; set; }
    }
}
