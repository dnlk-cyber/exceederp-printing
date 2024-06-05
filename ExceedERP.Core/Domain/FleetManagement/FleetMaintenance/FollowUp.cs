using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
