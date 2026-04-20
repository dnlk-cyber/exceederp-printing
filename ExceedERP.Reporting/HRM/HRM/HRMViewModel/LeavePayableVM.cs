using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeavePayableVM
    {
        public string FiscalYear { get; set; }
        public string FiscalYear1 { get; set; }
        public string FiscalYear2 { get; set; }
        public List<LeavePayableDetailVM> LeavePayableDetails { get; set; }
    }
    public class LeavePayableDetailVM
    {
        public string EmployeeFullName { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string FiscalYear1 { get; set; }
        public string FiscalYear2 { get; set; }
        public decimal Year1 { get; set; }
        public decimal Year2 { get; set; }
        public decimal TotalDays { get; set; }
        public decimal Payable { get; set; }

    }
}
