using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantScreeningResult
    {
        [Key]
        public int ScreeningResultId { get; set; }
        [Required]

        public int ApplicantRegistrationId { get; set; }
        public virtual ApplicantRegistration ApplicantRegistration { get; set; }
        [Required]

        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_Criteria", ResourceType = typeof(Localization.Resources))]
        public string Criteria { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_SettingValue", ResourceType = typeof(Localization.Resources))]

        public decimal SettingValue { get; set; }
        [Required]
        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_CurrentResult", ResourceType = typeof(Localization.Resources))]
        public decimal CurrentResult { get; set; }
        public string ReferenceType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public decimal TotalResult { get; set; }
        public int Rank { get; set; }
    }
}