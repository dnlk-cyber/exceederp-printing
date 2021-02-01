using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentForInternalApproval : TrackUserOperation
    {
        public int RentForInternalApprovalID { get; set; }
        public Guid RentInternalProjectId { get; set; }

        public int AssignedQuantity { get; set; }
        public virtual RentInternalProject RentInternalProject { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

 
}
