using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class VehicleDistributionApproval : TrackUserOperation
    {
        public int VehicleDistributionApprovalID { get; set; }
        public Guid VehicleDistributionId { get; set; }
        public virtual VehicleDistribution VehicleDistribution { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}
