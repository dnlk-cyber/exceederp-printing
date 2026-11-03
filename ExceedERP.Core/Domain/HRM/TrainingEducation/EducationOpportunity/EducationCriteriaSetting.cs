using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationCriteriaSetting: TrackUserOperation
    {
        public int EducationCriteriaSettingId { get; set; }
        public EducationCriteria Criteria { get; set; }
        public EducationPlanType Type { get; set; }
        public decimal Value { get; set; }
       

    }

}
