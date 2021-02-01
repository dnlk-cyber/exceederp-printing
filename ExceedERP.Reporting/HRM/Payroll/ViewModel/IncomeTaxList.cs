namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class IncomeTaxList
    {
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TotalOvertime { get; set; }
        public decimal TaxableAllowance { get; set; }
        public decimal TotalAdditions { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal TotalTaxable { get; set; }
        public decimal NetPay { get; set; }
        public string TIN { get; set; }
        public string Status { get; set; }
    }
}