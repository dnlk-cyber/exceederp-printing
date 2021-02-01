using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class PayrollInputs
    {
        public string BankAccountCode { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public int WorkingDays { get; set; }
      
        public decimal BasicSalary { get; set; }
        //awwce added
        public decimal TotalAbsent { get; set; }
      
        public decimal TotalTaxableAllowance { get; set; }
        public decimal TotalNonTaxableAllowance { get; set; }
        public decimal TotalTaxableAddition { get; set; }
        public decimal TotalNonTaxableAddition { get; set; }
        public decimal OverTime { get; set; }
        public decimal Saving { get; set; }
        public decimal Loan { get; set; }
        public decimal SportFee { get; set; }    
        public decimal LabourUnion { get; set; }
        public decimal Advance { get; set; }
        public List<Deductions> DeductionList { get; set; }
      
       
    }
}