using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
  public class EducationPlan: TrackUserSettingOperation
    {
        public int EducationPlanId { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Plan Type")]
        public EducationPlanType PlanType { get; set; }
        [Display(Name = "Category Type")]
        public EducationCategoryType CategoryType { get; set; }
        [Display(Name = "Education Field")]
        public int? EducationFieldId { get; set; }
        [Display(Name = "Education Level")]
        public int? EducationLevelId { get; set; }
        [Display(Name = " No of Employee")]
        public int Employee { get; set; }
    }
}
