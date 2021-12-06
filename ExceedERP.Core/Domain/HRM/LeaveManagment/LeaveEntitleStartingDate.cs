using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
   public class LeaveEntitleStartingDate
    {
        public int LeaveEntitleStartingDateId { get; set; }
        public LeaveEntitleStarting Start { get; set; }
        public string Remark { get; set; }
    }
    
}
