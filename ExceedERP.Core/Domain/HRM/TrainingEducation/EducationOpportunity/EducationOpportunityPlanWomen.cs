using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationPlanWomen
    {
        public int EducationPlanWomenId { get; set; }
        public int EducationPlanId { get; set; } // Foreign Key to EducationPlan
        public int EducationLevelId { get; set; }
        public int Quantity { get; set; } // Only female employees at this level
        //public virtual EducationOpportunityPlan EducationOpportunityPlan { get; set; }
    }
}
