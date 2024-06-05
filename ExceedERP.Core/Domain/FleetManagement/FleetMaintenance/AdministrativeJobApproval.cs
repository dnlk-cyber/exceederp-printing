using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class AdministrativeJobApproval : TrackUserOperation
    {
        public int AdministrativeJobApprovalID { get; set; }
        public Guid AdministrativeJobId { get; set; }
           public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}
