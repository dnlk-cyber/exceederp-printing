using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class SeverancePayableVM
    {
        public string FiscalYear { get; set; }
        public List<SeverancePayableDetailVM> SeverancePayablesDetails { get; set; }
    }
    public class SeverancePayableDetailVM
    {
        public string EmployeeCode { get; set; }
        public string EmployeeFullName { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public decimal Basicalary { get; set; }
        public string NomberOfYears { get; set; }
        public decimal GrossComposition { get; set; }
        public decimal GrossCompensationPayment { get; set; }
        public decimal Total { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal AccTaxDifference { get; set; }
        public decimal TaxOnMonthlySalaryAndCompensation { get; set; }
        public decimal TaxDifference { get; set; }
        public decimal NetPay { get; set; }

    }
}
