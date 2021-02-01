using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class PatientCardDetail
    {
        [Key]
        public int PatientCardDetailId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_PatientCardDetail_PatientCardId", ResourceType = typeof(Localization.Resources))]
        public int PatientId { get; set; }
        public virtual PatientCard PatientCard { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_PatientCardDetail_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_PatientCardDetail_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_PatientCardDetail_DiagnosisTratment", ResourceType = typeof(Localization.Resources))]
        public string DiagnosisTratment { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark;
        //public bool IsNew { get; set; }
        public bool IsSentToLaboratory { get; set; }
        public bool IsSentDespensary { get; set; }
        public bool IsTested { get; set; }
        public bool IsClosed { get; set; }

    }
}