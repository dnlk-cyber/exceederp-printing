using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public  class VehicleDemandApproval     : TrackUserOperation
    {
        public int VehicleDemandApprovalID { get; set; }
        public Guid VehicleDemandId { get; set; }
        public virtual VehicleDemand VehicleDemand { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}
