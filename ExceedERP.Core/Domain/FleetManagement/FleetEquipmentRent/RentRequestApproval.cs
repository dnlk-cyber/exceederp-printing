using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentRequestApproval : TrackUserOperation
    {
        public int RentRequestApprovalID { get; set; }
        public Guid RentRequestId { get; set; }
        public virtual RentRequest RentRequest { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}

