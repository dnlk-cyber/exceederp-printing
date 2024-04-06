using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingEvaluation
    {
        public int TrainingEvaluationId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_Criteria", ResourceType = typeof(Localization.Resources))]
        public int CriteriaId { get; set; }
        [Display(Name = "HR_TrainingEducation_Education_Score", ResourceType = typeof(Localization.Resources))]
        public TrainingScore Score { get; set; }
        public int EmployeeId { get; set; }
        public int CourseTrackingId { get; set; }
    }
}
