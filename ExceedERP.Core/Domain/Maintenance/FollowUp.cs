using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class FollowUp : TrackUserSettingOperation
    {
        public Guid FollowUpId { get; set; }
        public Guid ServiceCrewWorkLogId { get; set; }
        public Guid JobOrderId { get; set; }
        public string DurationHr { get; set; }
        public string Status { get; set; }

         
              }
}
