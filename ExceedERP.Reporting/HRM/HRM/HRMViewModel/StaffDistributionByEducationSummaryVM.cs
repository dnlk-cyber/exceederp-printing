namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class StaffDistributionByEducationSummaryVM
    {
        public string ProjectName { get; set; }

        public int PermanentMale { get; set; }
        public int PermanentFemale { get; set; }
        public int PermanentTotal => PermanentMale + PermanentFemale;

        public int ContractMale { get; set; }
        public int ContractFemale { get; set; }
        public int ContractTotal => ContractMale + ContractFemale;

        public int TemporaryMale { get; set; }
        public int TemporaryFemale { get; set; }
        public int TemporaryTotal => TemporaryMale + TemporaryFemale;

        public int GrandMale => PermanentMale + ContractMale + TemporaryMale;
        public int GrandFemale => PermanentFemale + ContractFemale + TemporaryFemale;
        public int GrandTotal => GrandMale + GrandFemale;
    }
}
