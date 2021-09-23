using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentFromExternalApproval : TrackUserOperation
    {
        public int RentFromExternalApprovalID { get; set; }
        public Guid RentFromExternalId { get; set; }
        public virtual RentFromExternal RentFromExternal { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }


}

