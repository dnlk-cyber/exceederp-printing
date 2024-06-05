using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class AdministrativeJobApproval : TrackUserOperation
    {
       public int AdministrativeJobApprovalID { get; set; }
       public Guid AdministrativeJobId { get; set; }
       public ApprovalStatuses Status { get; set; }
       public string Remark { get; set; }
    }


}
