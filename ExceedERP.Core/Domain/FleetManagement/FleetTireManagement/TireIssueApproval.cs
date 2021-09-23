using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{
    public class TireIssueApproval : TrackUserOperation
    {
        public int TireIssueApprovalID { get; set; }
        public Guid TireIssueId { get; set; }
        public virtual TireIssue TireIssue { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}