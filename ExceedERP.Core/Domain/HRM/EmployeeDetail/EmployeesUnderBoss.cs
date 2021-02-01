using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
   public class EmployeesUnderBossVM
    {
        public int EmployeesUnderBossId { get; set; }
        public string FullName {get; set; }
        public string  EmployeeCode { get; set; }
        public bool IsCurrent { get; set; }
    }
}
