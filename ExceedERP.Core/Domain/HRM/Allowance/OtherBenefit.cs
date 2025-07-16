using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class OtherBenefit:TrackUserOperation 
    {
        [Key]
        public int OtherBenefitId { get; set; }
        [Required ]
        [Display(Name = "HR_Allowance_OtherBenefit_JobTitleId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required ]
        [Display(Name = "HR_Allowance_OtherBenefit_BenefitName", ResourceType = typeof(Localization.Resources))]
        public int BenefitName { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_OtherBenefit_UnitofMeasurement", ResourceType = typeof(Localization.Resources))]
        public int UnitofMeasurement { get; set; }
        [Required ]
        [Display(Name = "HR_Allowance_OtherBenefit_Quantity", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Quantity { get; set; }
        [Display(Name = "HR_Allowance_OtherBenefit_TimeInterval", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public string TimeInterval { get; set; }
        [Display(Name = "Is For All")]
        public bool IsForAll { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public GenderBenefit Gender { get; set; }

    }
    public enum GenderBenefit
    {
        [Display(Name = "Enum_Male", ResourceType = typeof(Resources))]
        MALE = 5,
        [Display(Name = "Enum_Female", ResourceType = typeof(Resources))]
        FEMALE = 6,
        BOTH

    }
}