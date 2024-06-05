using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
   public class EquipmentArrivalApproval : TrackUserSettingOperation
    {
       public Guid EquipmentArrivalApprovalId { get; set; }
        public Guid EquipmentArrivalId { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public virtual EquipmentArrival EquipmentArrivals { get; set; }
    }
}
