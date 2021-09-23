namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class ProbationPerformanceSetting
    {
        public int ProbationPerformanceSettingId { get; set; }
        public int IntialWeight { get; set; }
        public int EndWeight { get; set; }
        public ProbationMeasurement ProbationMeasurement { get; set; }
    }
}
