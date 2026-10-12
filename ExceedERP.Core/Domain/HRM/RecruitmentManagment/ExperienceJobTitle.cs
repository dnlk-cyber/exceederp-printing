using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ExperienceJobTitle: TrackUserOperation
    {
        public int ExperienceJobTitleId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
    public class ExperienceJobTitleDetail : TrackUserOperation
    {
        public int ExperienceJobTitleDetailId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "ExamType", ResourceType = typeof(Localization.Resources))]
        public ExamType ExamType { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        public int ExperienceJobTitleId { get; set; }
        public virtual ExperienceJobTitle ExperienceJob { get; set; }
    }

    public class EducationFieldDetail : TrackUserOperation
    {
        public int EducationFieldDetailId { get; set; }
        [Display(Name = "Education Field")]
        public int LookupId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        public int ExperienceJobTitleId { get; set; }
        public virtual ExperienceJobTitle ExperienceJob { get; set; }
    }
    public class RelatedEducationFieldDetail : TrackUserOperation
    {
        public int RelatedEducationFieldDetailId { get; set; }
        [Display(Name = "Related Education Field")]
        public int LookupId { get; set; }
        
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        public int ExperienceJobTitleId { get; set; }
        public virtual ExperienceJobTitle ExperienceJob { get; set; }
    }
    public class EducationLevelDetail : TrackUserOperation
    {
        public int EducationLevelDetailId { get; set; }
        [Display(Name = "Education Level")]
        public int LookupId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        public decimal Experience { get; set; }
        public int ExperienceJobTitleId { get; set; }
        public virtual ExperienceJobTitle ExperienceJob { get; set; }
    }


}
