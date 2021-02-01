using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
   public class MonthlyLeavePlan : TrackUserOperation
    {
        public int MonthlyLeavePlanId { get; set; }
        public int EmployeeId { get; set; }
        public int GlFiscalYearId { get; set; }
        
    }
}
