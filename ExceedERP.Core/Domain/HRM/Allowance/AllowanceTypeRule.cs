using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class AllowanceTypeRule :TrackUserOperation 
    {
        [Key]
        public int AllowanceTypeRuleId { get; set; }
        [Required ]
        [Display(Name = "HR_Allowance_AllowanceTypeRule_AllowanceType", ResourceType = typeof(Localization.Resources))]
        public AllowanceType AllowanceType { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "HR_Allowance_AllowanceTypeRule_TaxTrasholdAmount", ResourceType = typeof(Localization.Resources))]
        public decimal TaxTrasholdAmount { get; set; }
        [Display(Name = "HR_Allowance_AllowanceTypeRule_Percentage", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Percentage { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
         

    }
}