using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class MaintenanceType : TrackUserSettingOperation
    {
        public Guid MaintenanceTypeId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
