namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class LoanReport
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public decimal LoanAmount { get; set; }
        public string StartingMonth { get; set; }
        public string EndingMonth { get; set; }
        public int RemainingMonths { get; set; }
        public decimal Month { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal Balance { get; set; }
        public string IsActive { get; set; }
        public string Description { get; set; }
    }
}