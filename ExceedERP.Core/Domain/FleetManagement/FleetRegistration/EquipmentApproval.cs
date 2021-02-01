using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
        public  class EquipmentApproval : TrackUserSettingOperation
    {
            public int EquipmentApprovalId { get; set; }
            public Guid FleetsId { get; set; }
            public string  Status { get; set; }
            public string Remark { get; set; }

            public virtual Fleets Fleets { get; set; }

    }
}
