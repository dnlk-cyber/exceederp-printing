using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
   public class LeaveUsageTrackingForVoid
    {
        public int LeaveUsageTrackingForVoidId { get; set; }
        public int EmployeeId { get; set; }
        public int GlFisicalYearId { get; set; }
        public int Index { get; set; }
        public decimal Amount { get; set; }
        public int LeaveRequestId { get; set; }
        public bool IsActive { get; set; }
    }
}
