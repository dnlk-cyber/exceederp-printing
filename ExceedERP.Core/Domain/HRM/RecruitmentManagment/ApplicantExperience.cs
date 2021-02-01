using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantExperience
    {
        [Key]
        public int ApplicantExpId { get; set; }

        public int ApplicantRegistrationId { get; set; }
        public virtual ApplicantRegistration ApplicantRegistration { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_EduLevel", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EduLevel { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_EducField", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EducField { get; set; }
        
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_OrganizationName", ResourceType = typeof(Localization.Resources))]
        public string OrganizationName { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_Postion", ResourceType = typeof(Localization.Resources))]
        public string Postion { get; set; }

        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_Experience", ResourceType = typeof(Localization.Resources))]
        public decimal Experience { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_ToAmharic", ResourceType = typeof(Localization.Resources))]
        public string ToAmahric { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}