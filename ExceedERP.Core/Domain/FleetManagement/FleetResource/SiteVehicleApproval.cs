using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class SiteVehicleApproval : TrackUserOperation
    {
        public int SiteVehicleApprovalID { get; set; }
        public Guid SitesVehicleTransferId { get; set; }
        public virtual SitesVehicleTransfer SitesVehicleTransfer { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }


}