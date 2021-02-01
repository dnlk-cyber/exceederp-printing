namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class AdvanceReport
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal Month { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal Balance { get; set; }
        public string IsActive { get; set; }
        public string Description { get; set; }
        public string StartingMonth { get; internal set; }
        public int RemainingMonths { get; internal set; }
    }
}