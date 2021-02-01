using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;


namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class InsuranceCategory : TrackUserOperation
    {
        [Key]
        public int InsuranceCategoryId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_InsuranceCategory_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        //[Required]
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

        [Required]
        [Display(Name = "HR_HealthAndInsurance_InsuranceCategory_Name", ResourceType = typeof(Localization.Resources))]
        public int InsuranceTypeId { get; set; }
        public virtual InsuranceType InsuranceType { get; set; }

    }
}