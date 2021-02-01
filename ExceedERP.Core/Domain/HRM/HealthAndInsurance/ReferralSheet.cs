using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class ReferralSheet
    {
        [Key]
        public int ReferralSheetId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_PatientCardId", ResourceType = typeof(Localization.Resources))]
        public int PatientId { get; set; }
        public virtual PatientCard PatientCard { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_ReferredFrom", ResourceType = typeof(Localization.Resources))]
        public string ReferredFrom { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_ReferredTo", ResourceType = typeof(Localization.Resources))]
        public int MainCompanyId { get; set; }
        public virtual MainCompany MainCompany { get; set; }
      
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_BriefHistory", ResourceType = typeof(Localization.Resources))]
        public string BriefHistory { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_PhiscalFindings", ResourceType = typeof(Localization.Resources))]
        public string PhiscalFindings { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_LabXrayFindings", ResourceType = typeof(Localization.Resources))]
        public string LabXrayFindings { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_Diagnosis", ResourceType = typeof(Localization.Resources))]
        public string Diagnosis { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_TreatmentGiven", ResourceType = typeof(Localization.Resources))]
        public string TreatmentGiven { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_Recommendation", ResourceType = typeof(Localization.Resources))]
        public string Recommendation { get; set; }
         
        [Display(Name = "HR_HealthAndInsurance_ReferralSheet_RecommendedBy", ResourceType = typeof(Localization.Resources))]
        public string RecommendedBy { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}