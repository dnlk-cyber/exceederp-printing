using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class TimeSheetApproval : TrackUserOperation
    {
       public int TimeSheetApprovalId { get; set; }
       public Guid TimeSheetId { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }
        public virtual TimeSheet TimeSheets { get; set; }
    }
}
