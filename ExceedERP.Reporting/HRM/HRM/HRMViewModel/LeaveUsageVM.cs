using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeaveUsageVM
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Title { get; set; }
        public string FiscalYear { get; set; }

        public Dictionary<string, decimal> FiscalYearBalances { get; set; } = new Dictionary<string, decimal> ();

        public decimal Balance { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
    }
    //public class LeaveUsageVM
    //{
    //     public string EmployeeCode { get; set; }
    //     public string EmployeeName { get; set; }
    //     public string Title { get; set; }
    //     public string FiscalYear1 { get; set; }
    //     public decimal Balance1 { get; set; }
    //     public string FiscalYear2 { get; set; }
    //     public decimal Balance2 { get; set; }
    //     public string FiscalYear3 { get; set; }
    //     public decimal Balance3 { get; set; }
    //     public decimal TotlalAmount { get; set; }
    //     public string Remark { get; set; }

    // }


}
