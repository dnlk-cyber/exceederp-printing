using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }
        [Required ]
        [Display(Name = "HR_HealthAndInsurance_Treatment_PrescriptionId", ResourceType = typeof(Localization.Resources))]
        public int PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Treatment_Measure", ResourceType = typeof(Localization.Resources))]
        public string Measure { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Treatment_Quntity", ResourceType = typeof(Localization.Resources))]
        public decimal Quntity { get; set; }
    }
}