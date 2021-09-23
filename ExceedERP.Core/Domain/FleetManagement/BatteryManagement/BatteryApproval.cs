using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.BatteryManagement
{
    public class BatteryApproval : TrackUserOperation
    {
        public Guid BatteryApprovalId { get; set; }
        public Guid BatteryId { get; set; }
        
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }

}