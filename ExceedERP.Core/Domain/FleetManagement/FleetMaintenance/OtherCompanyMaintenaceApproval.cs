using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public  class OtherCompanyMaintenaceApproval : TrackUserOperation
    {
        public int OtherCompanyMaintenaceApprovalID { get; set; }
        public Guid OtherCompanyMaintenaceId { get; set; }
        public virtual OtherCompanyMaintenance OtherCompanyMaintenace { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }


}
