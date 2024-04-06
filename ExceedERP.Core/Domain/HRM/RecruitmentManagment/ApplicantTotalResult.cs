using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantTotalResult
    {
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Name))]
        public string Name { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Age))]
        public int Age { get; set; }
       // [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public Gender Sex { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.EducationLevel))]
        public decimal EduLevel { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.EducationField))]
        public decimal EduField { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Enum_RecruitmentSetting_RELEVANT_EXPERIENCE))]
        public decimal RelevantExperience { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Enum_RecruitmentSetting_RELATED_EXPERIENCE))]
        public decimal RelatedExperience { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public decimal Gender { get; set; }    
        public decimal Disability { get; set; }  
        public decimal Profile { get; set; }  
        public decimal Training { get; set; } 
        public decimal Interview { get; set; }
        public decimal TheoryExam { get; set; } 
        public decimal PracticalExam { get; set; }
        public decimal Discipline { get; set; }  
        public decimal Total { get; set; }
        public decimal Nation { get; set; }
        public int Rank { get; set; }
        public decimal Performance { get; set; }
        public RecruitmentResult RecruitmentResult { get; set; }
    }
}
