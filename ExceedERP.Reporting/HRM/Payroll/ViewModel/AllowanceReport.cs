using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.Payroll.ViewModel
{
    public class AllowanceReport
    {
        public string EmpID { get; set; }
        public string EmpFullName { get; set; }
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public decimal BasicSalary { get; set; }
        public List<AllowanceTaxable> AllowanceTaxableList { get; set; }

        public List<AllowanceNonTax> AllowanceNonTaxList { get; set; }
    }
}