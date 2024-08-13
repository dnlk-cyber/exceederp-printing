using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.HRM.SelfCare;

namespace ExceedERP.Core.Domain.Health
{
    public class ReferralForm
    {
        [Key]
        public int ReferralFormId { get; set; }
        [Required ]
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_MedicalRequestId", ResourceType = typeof(Localization.Resources))]
        public int MedicalRequestId { get; set; }
        public virtual MedicalRequest MedicalRequest { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_ReferredDate", ResourceType = typeof(Localization.Resources))]
        public DateTime ReferredDate { get; set; }
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_ReferredDate", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string ReferredDateAmharic { get; set; }
        [DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{HH:MM}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_ReferredTime", ResourceType = typeof(Localization.Resources))]
        public DateTime ReferredTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
       
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_ReturnedDate", ResourceType = typeof(Localization.Resources))]
        public DateTime? ReturnedDate { get; set; }
     
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_ReturnedDate", ResourceType = typeof(Localization.Resources))]
        public string ReturnedDateAmharic { get; set; }
        [DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{ HH:MM}")]
       
        [Display(Name = "HR_HealthAndInsurance_ReferralForm_Time", ResourceType = typeof(Localization.Resources))]
        public DateTime? ReturnedTime { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}