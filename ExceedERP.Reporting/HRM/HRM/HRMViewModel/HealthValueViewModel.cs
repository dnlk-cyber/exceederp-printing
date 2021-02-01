using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class HealthValueViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public int GlFiscalYearId { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public List<HealthValueListViewModel> HealthValueList { get; set; }
    }
}
