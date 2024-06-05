using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
