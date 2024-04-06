using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingNeedRequest
   {
        [Key]
        public int TrainingNeedRequestId { get; set; }
        [Display(Name ="Employee Name")]
        public int EmployeeId { get; set; }
        [Required]
        public string Prerequest { get; set; }
        [Display(Name = "Employee's Gap")]
        [Required]
        public string EmployeePerformanceGap { get; set; }
        [Display(Name ="Training need for Gap")]
        [Required]
        public string TrainingNeedForEmployeePerformanceGap { get; set; }
        [Display(Name = "Training related to Inistitution")]
        public string TheNeedForTrainingRelatedToInistitution { get; set; }
        [Display(Name = "Expected from  Training")]
        public string ExpectedResultFromTraining { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_OrganizationId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "Enum_LookUpTypes_TARGET_GROUP", ResourceType = typeof(Localization.Resources))]
        public int RequiredTraining { get; set; }
        [Display(Name = "HR_TrainingEducation_NeedAssessment_Course", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public int GlFiscalYearId { get; set; }
        public int AssesmentId { get; set; }
        public NeedAssessment assessment { get; set; }
   }
    public class TrainingEvaluationBefore
    {
        public int TrainingEvaluationBeforeId { get; set; }
        public string Name { get; set; }
        public int TrainingEvaluationCriteriaId { get; set; }
        public int TrainingEvaluationSettingCategoryId { get; set; }
        [Display(Name = "Rpt_Criteria", ResourceType = typeof(Localization.Resources))]
        public string Criteria { get; set; }
        public decimal Weight { get; set; }
        public string Result { get; set; }
        public int TrainingNeedRequestId { get; set; }
        public TrainingNeedRequest NeedRequest { get; set; }

    }
    public class TrainingEvaluationAfter
    {
        public int TrainingEvaluationAfterId { get; set; }
        public string Name { get; set; }
        public int TrainingEvaluationCriteriaId { get; set; }
        public int TrainingEvaluationSettingCategoryId { get; set; }
        [Display(Name = "Rpt_Criteria", ResourceType = typeof(Localization.Resources))]
        public string Criteria { get; set; }
        public decimal Weight { get; set; }
        public string Result { get; set; }
        public int TrainingNeedRequestId { get; set; }
        public TrainingNeedRequest NeedRequest { get; set; }

    }
}
