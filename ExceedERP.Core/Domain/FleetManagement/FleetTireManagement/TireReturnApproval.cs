using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{
    public class TireReturnApproval : TrackUserOperation
    {
        public int TireReturnApprovalID { get; set; }
        public Guid TireReturnId { get; set; }
        public virtual TireReturn TireReturn { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}