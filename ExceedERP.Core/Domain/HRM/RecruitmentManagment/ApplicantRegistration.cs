using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantRegistration : TrackUserOperation
    {

        [Key]
        public int ApplicantRegistrationId { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        public virtual Vacancy Vacancy { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_ApplicantName", ResourceType = typeof(Localization.Resources))]
        public string ApplicantName { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_Age", ResourceType = typeof(Localization.Resources))]
        public int Age { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_Sex", ResourceType = typeof(Localization.Resources))]
        public Gender Sex { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_RegistrationDate", ResourceType = typeof(Localization.Resources))]
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_RegistrationDate", ResourceType = typeof(Localization.Resources))]
        public string RegistrationDateAm { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_RegistrationPalce", ResourceType = typeof(Localization.Resources))]
        public string RegistrationPalce { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_RegisterBy", ResourceType = typeof(Localization.Resources))]
        public string RegisterBy { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_TotalResult", ResourceType = typeof(Localization.Resources))]
        public decimal TotalResult { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VacancyType", ResourceType = typeof(Localization.Resources))]
        public VacancyType VacancyType { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_VaccancyCode", ResourceType = typeof(Localization.Resources))]
        public string VacancyCode { get; set; }
        [Display(Name = "HR_HRMPlan_RecruitmentPlan_JobtitleName", ResourceType = typeof(Localization.Resources))]
        public string JobtitleName { get; set; }
        public int Rank { get; set; }
        public bool Check { get; set; }
        [Display(Name = "Applicant_Registration_Fname", ResourceType = typeof(Localization.Resources))]
        public string FirstName { get; set; }
        [Display(Name = "Applicant_Registration_Mname", ResourceType = typeof(Localization.Resources))]
        public string MiddleName { get; set; }
        [Display(Name = "Applicant_Registration_Lname", ResourceType = typeof(Localization.Resources))]
        public string LastName { get; set; }
        public bool IsTransfered { get; set; }
        public string MediaVacancyCode { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantRegistration_LookupId", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public string IDAndTitle { get; set; }

    }
}