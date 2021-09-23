using System;
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

