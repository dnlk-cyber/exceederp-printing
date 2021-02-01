using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class MonthlySalaryPerPeriodVM
    {
        public string EmpID { get; set; }
        public string EmpFullName { get; set; }
        public string BankAccount { get; set; }
        public decimal NetEarning { get; set; }
        public string Rank { get; set; }
        public string Step { get; set; }
        public string JobTitle { get; set; }

    }
}
