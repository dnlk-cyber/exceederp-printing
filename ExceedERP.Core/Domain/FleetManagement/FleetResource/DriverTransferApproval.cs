using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class DriverTransferApproval : TrackUserOperation
    {
        public int DriverTransferApprovalID { get; set; }
        public Guid DriverVehicleTransferId { get; set; }
        public virtual DriverVehicleTransfer DriverVehicleTransfer { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }


}
