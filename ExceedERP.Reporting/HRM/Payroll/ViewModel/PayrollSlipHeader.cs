namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class PayrollSlipHeader
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string PeriodName { get; set; }
        public decimal NetPay { get; set; }
        public string Company { get; set; }
    }
}