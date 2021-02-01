using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class EmployeeLeavePlan
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public decimal Total { get; set; }
        public List<LeavePlan> LeavePlans { get; set; }
    }
}
