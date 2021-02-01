using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class AccidentMedicalRecord
    {
        [Key]
        public int AccidentMedicalRecordId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentMedicalRecord_AccidentRecordId", ResourceType = typeof(Localization.Resources))]
        public int AccidentRecordId { get; set; }
        public virtual AccidentRecord AccidentRecord { get; set; }
        
        [Display(Name = "HR_HealthAndInsurance_AccidentMedicalRecord_MedicalTreatment", ResourceType = typeof(Localization.Resources))]
        public string MedicalTreatment { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentMedicalRecord_InstitutionName", ResourceType = typeof(Localization.Resources))]
        public string InstitutionName { get; set; }
        [Display(Name = "HR_HealthAndInsurance_AccidentMedicalRecord_MedicalCost", ResourceType = typeof(Localization.Resources))]
        public decimal MedicalCost { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Ramrk { get; set; }
        
    }
}