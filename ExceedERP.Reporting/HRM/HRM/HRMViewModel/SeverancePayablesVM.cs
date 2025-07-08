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
        public string EmployeeFullName { get; set; }
        public decimal Basicalary { get; set; }
        public string NomberOfYears { get; set; }
        public decimal GrossComposition { get; set; }
        public decimal Total { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal TotalIncomeTax { get; set; }
        public decimal Deduction { get; set; }
        public decimal TotalDeuduction { get; set; }
        public decimal NetPay { get; set; }

    }
}
