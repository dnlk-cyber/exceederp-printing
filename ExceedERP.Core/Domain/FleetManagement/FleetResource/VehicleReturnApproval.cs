using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class VehicleReturnApproval : TrackUserOperation
    {
        public int VehicleReturnApprovalID { get; set; }
        public Guid VehicleReturnId { get; set; }
        public virtual VehicleReturn VehicleReturn { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}
