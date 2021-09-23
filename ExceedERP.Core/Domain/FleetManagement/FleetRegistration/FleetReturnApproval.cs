using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class FleetReturnApproval : TrackUserOperation
    {
       public int FleetReturnApprovalId { get; set; }
       public int FleetReturnId { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; } 
    }
}
