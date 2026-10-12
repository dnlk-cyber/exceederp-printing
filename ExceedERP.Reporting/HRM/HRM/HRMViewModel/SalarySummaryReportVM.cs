using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class SalarySummaryReportVM
    {
        public string Status { get; set; }
        public int NumberOfEmployee { get; set; }
        public decimal SumOfGrossSalary { get; set; }
        public decimal PositionAllowance { get; set; }
        public decimal ResponsibilityAllowance { get; set; }
        public decimal OT { get; set; }
    }

}
