namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class PayrollDetailViewmodel
    {
        public string EmpID { get; set; }
        public string EmpFullName { get; set; }
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public decimal BasicSalary { get; set; }
        //awwce added
        public decimal TotalAbsent { get; set; }
        public decimal Overtime { get; set; }
        public decimal TotalTaxableAllowance { get; set; }
        public decimal TotalNonTaxableAllowance { get; set; }
       
        public decimal TotalTaxablePay { get; set; }
        public decimal TotalNonTaxablePay { get; set; }
        //
        public decimal IncomeTax { get; set; }
        public decimal Pension { get; set; }
        public decimal Provident { get; set; }
        public decimal ProvidentPension { get; set; }
        public decimal Loan { get; set; }
        public decimal SavingCredit { get; set; }
        public decimal Advance { get; set; }
        public decimal LabourUnion { get; set; }
        public decimal BankLoan { get; set; }
        public decimal OtherDeduction { get; set; }
        public decimal OtherAddition { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetPay { get; set; }
        public string BankAccount { get; set; }
        public string Department { get; set; }
        public int WorkingDays { get; set; }
        public decimal Saving { get; set; }
      
        public decimal SportFee { get; set; }
        public decimal NonTaxAddition { get; set; }
        public string WorkUnitName { get; set; }
    }
}