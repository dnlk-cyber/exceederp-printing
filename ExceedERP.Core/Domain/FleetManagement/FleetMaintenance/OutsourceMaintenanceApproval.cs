using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class OutsourceMaintenanceApproval : TrackUserOperation
    {
        public int OutsourceMaintenanceApprovalId { get; set; }
        public Guid JobOrderId { get; set; }

        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public virtual JobOrder JobOrder { get; set; }
    }


}
