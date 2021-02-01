using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class LanguageProficiency :TrackUserOperation 
    {
        public int LanguageProficiencyId { get; set; }
        [Display(Name = "Common_LanguageProficiency_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }

        public virtual Person_Employee Employee { get; set; } 
       
        [Display(Name = "Common_LanguageProficiency_LanguageId", ResourceType = typeof(Localization.Resources))]
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
        [Display(Name = "Common_LanguageProficiency_Reading", ResourceType = typeof(Localization.Resources))]
        public Proficiency Reading { get; set; }
        [Display(Name = "Common_LanguageProficiency_Listening", ResourceType = typeof(Localization.Resources))]
        public Proficiency Listening { get; set; }
        [Display(Name = "Common_LanguageProficiency_Writing", ResourceType = typeof(Localization.Resources))]
        public Proficiency Writing { get; set; }
        [Display(Name = "Common_LanguageProficiency_Speaking", ResourceType = typeof(Localization.Resources))]
        public Proficiency Speaking { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}