namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    // One row per (Project, EducationLevel) - feeds the Crosstab so its columns are generated
    // directly from whatever LookUpTypes.EDUCATION_LEVEL entries exist, instead of a fixed set.
    public class StaffDistributionByEducationLevelCrosstabRowVM
    {
        public string ProjectName { get; set; }
        public string EducationLevelName { get; set; }
        public int EducationLevelIndex { get; set; }
        public int Count { get; set; }
    }
}
