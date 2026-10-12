using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters.ViewModel
{
    public class TerminationVM
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string ApplicationDate { get; set; }
        public string RequestDate { get; set; }
        public string TerminationReason { get; set; }
        public string JobTitle { get; set; }
        public string Branch { get; set; }

        public string BusinessUnit { get; set; }
        public string Grade { get; set; }
        public string TeminationDateRequest { get; set; }
        public string TeminatedDate { get; set; }
        public string TeminatedDateAm { get; set; }
        public decimal Salary { get; set; }
        public string  SalaryInWord { get; set; }
        public string EmployeementType { get; set; }

    }
}
