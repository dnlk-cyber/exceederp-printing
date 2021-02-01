using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class ClinicViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Branch { get; set; }
        public string Workunit { get; set; }
        public string TreatedDate { get; set; }
        public string ReferalDate { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Diagnosis { get; set; }
    }
}
