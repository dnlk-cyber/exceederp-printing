using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantEducation
    {
        [Key]
        public int ApplicantEducationId { get; set; }

        public int ApplicantRegistrationId { get; set; }
        public virtual ApplicantRegistration ApplicantRegistration { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_EduLevel", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EduLevel { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_EducField", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EducField { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_EducationInstitution", ResourceType = typeof(Localization.Resources))]
        public string EducationInstitution { get; set; }
        
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //[Display(Name = "HR_RecruitmentManagment_ApplicantExperience_From", ResourceType = typeof(Localization.Resources))]
        //public DateTime From { get; set; }
        //[Display(Name = "HR_RecruitmentManagment_ApplicantExperience_From", ResourceType = typeof(Localization.Resources))]
        //public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantExperience_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmahric { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}