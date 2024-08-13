using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Health
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Prescription_PatientCardId", ResourceType = typeof(Localization.Resources))]
        public int PatientId { get; set; }

        public virtual PatientCard PatientCard { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Prescription_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Prescription_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Prescription_Rate", ResourceType = typeof(Localization.Resources))]
        public string Rate { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Prescription_TreatedBy", ResourceType = typeof(Localization.Resources))]
        public string TreatedBy { get; set; }
       
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]


        public string Remark { get; set; }
    }
}