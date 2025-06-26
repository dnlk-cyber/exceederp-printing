using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
     public class InternalJobToExternal : Operations
    {
        public int InternalJobToExternalId { get; set; }
        public string ServiceCenterJobId { get; set; }
        public string Description { get; set; }
    }

    public class InternalJobToExternalValidation : TrackUserSettingOperation
    {
        public int InternalJobToExternalValidationId { get; set; }
        public int InternalJobToExternalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
