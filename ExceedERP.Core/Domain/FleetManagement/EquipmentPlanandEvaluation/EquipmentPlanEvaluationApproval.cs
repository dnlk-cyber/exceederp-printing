using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation
{
    public class EquipmentPlanEvaluationApproval : TrackUserOperation
    {
        public int EquipmentPlanEvaluationApprovalId { get; set; }
        public Guid EquipmentPlanandEvaluationsId { get; set; }
   
        public string Status { get; set; }
        public string Remark { get; set; }
        public virtual EquipmentPlanandEvaluations EquipmentPlanandEvaluations { get; set; }
    }
}
