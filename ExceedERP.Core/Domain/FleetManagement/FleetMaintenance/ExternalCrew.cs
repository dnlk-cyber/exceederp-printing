 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ExternalCrew : TrackUserSettingOperation
    {

        public Guid ExternalCrewID { get; set; }
        public Guid JobId { get; set; }
        public string Name { get; set; }
        public ApprovalStatuses ApprovalStatuses { get; set; }
    }
}
