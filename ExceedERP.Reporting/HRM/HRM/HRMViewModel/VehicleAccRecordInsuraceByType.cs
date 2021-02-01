namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class VehicleAccRecordInsuraceByType
    {
        public string Type { get; set; }
        public HighLevelAccident HighLevelAccidents { get; set; }
        public MediumLevelAccident MediumLevelAccidents { get; set; }
        public LowLevelAccident LowLevelAccidents { get; set; }
    }
}