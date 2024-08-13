using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance
{
    public class Restitution
    {
        [Key ]
        public int RestitutionId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Restitution_Accident", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int AccidentRecordId { get; set; }
     
        public virtual AccidentRecord AccidentRecord { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Restitution_RestitutionAmount", ResourceType = typeof(Localization.Resources))]
        public decimal RestitutionAmount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}