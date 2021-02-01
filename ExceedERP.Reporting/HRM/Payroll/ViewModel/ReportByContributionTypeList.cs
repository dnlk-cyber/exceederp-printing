namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class ReportByContributionTypeList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal Balance { get; set; }
        public string Branch { get; set; }
        public string Period { get; set; }
        public string ContributionName { get; set; }
        public string Source { get; set; }
    }
}