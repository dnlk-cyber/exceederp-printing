using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation
{
    public class PlanAndEvaluationApproval : TrackUserOperation
    {
        public int PlanAndEvaluationApprovalID { get; set; }
        public Guid EquipmentPlanandEvaluationsId { get; set; }
        public virtual EquipmentPlanandEvaluations EquipmentPlanandEvaluations { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}
