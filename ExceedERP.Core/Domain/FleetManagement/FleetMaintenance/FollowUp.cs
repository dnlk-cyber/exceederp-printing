using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class FollowUp : TrackUserSettingOperation
    {
        public Guid FollowUpId { get; set; }
        public Guid JobOrderId { get; set; }
        public string DurationHr { get; set; }
        public string Status { get; set; }

         
              }
}
