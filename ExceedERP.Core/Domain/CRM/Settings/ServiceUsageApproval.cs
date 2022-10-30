using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class ServiceUsageApproval : TrackUserOperation
    {

            public int ServiceUsageApprovalId { get; set; }
            public int ServiceUsageId { get; set; }
            public string Status { get; set; }
            public string Remark { get; set; }
            public virtual ServiceUsage ServiceUsage { get; set; }
    }
}
