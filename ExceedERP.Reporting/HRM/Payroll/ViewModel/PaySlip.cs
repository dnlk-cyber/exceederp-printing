using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Payroll;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class PaySlip
    {
        public string Company { get; set; }
        public string EmpID { get; set; }
        public string EmpFullName { get; set; }
        public string PayrollPeriodName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Overtime { get; set; }
        public decimal NetPay { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal PensionContribution { get; set; }
        public decimal ProvidentContribution { get; set; }
        public decimal CreditAssociation { get; set; }
        public decimal LabourUnion { get; set; }
        public decimal TotalEarning { get; set; }
        public decimal TotalDeduction { get; set; }
        public int Index { get; set; }
        public List<PayrollProcessAllowances> PayrollProcessAllowances { get; set; }
        public List<PayrollProcessAdditions> PayrollProcessAdditions { get; set; }
        public List<PayrollProcessDeductions> PayrollProcessDeductions { get; set; }
        public List<PayrollProcessLongTermDeductions> PayrollProcessLongTermDeductions { get; set; }
        public List<PayrollProcessLoans> PayrollProcessLoans { get; set; }
        public List<PayrollProcessAdvances> PayrollProcessAdvances { get; set; }
        public List<PayrollProcessFixedContribution> PayrollProcessFixedContribution { get; set; }
        public List<PayrollProcessRegularContribution> PayrollProcessRegularContribution { get; set; }
        public decimal WorkingDays { get; set; }
        public string Deparatment { get; set; }
        public string BranchName { get; set; }
        public string FiscalYear { get; set; }
        public decimal GrossEarning { get; set; }
        public decimal GrossDeduction { get; set; }
        public decimal Months { get; set; }
    }
}