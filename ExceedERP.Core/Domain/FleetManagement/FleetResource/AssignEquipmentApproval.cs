using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class AssignEquipmentApproval : TrackUserOperation
    {
        public int AssignEquipmentApprovalID { get; set; }
        public Guid AssignEquipmentId { get; set; }
        public virtual AssignedEquipment AssignedEquipment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}
