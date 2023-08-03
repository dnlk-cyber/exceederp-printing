using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Plan
{
    public class ProcurementPlanValidation : TrackUserOperation
    {
        public int ProcurementPlanValidationId { get; set; }
        public int ProcurementPlanId { get; set; }
        public virtual ProcurementPlan ProcurementPlan { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
