using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    public class EmployeeViewModel
    {
        public int EmpId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string FullAmharicName { get; set; }
        public string EmpCodeWithName { get; set; }
        public string EmpCodeWithNameEng { get; set; }
    }
}
