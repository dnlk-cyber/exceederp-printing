using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    /// <summary>    An allowance defination. </summary>
    ///
    /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AllowanceDefination : TrackUserOperation
    {
        [Key]
        public int AllowanceDefinationId { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowanceDefination_AllowanceName", ResourceType = typeof(Localization.Resources))]
        public string AllowanceName { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowanceDefination_AllowanceType", ResourceType = typeof(Localization.Resources))]
        public AllowanceType AllowanceType { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowanceDefination_Amount", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Amount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        // [Display(Name = "HR_Allowance_AllowanceDefination_LookupId", ResourceType = typeof(Localization.Resources))]
        //public int? LookupId { get; set; }
        [Display(Name = "HR_Allowance_AllowanceDefination_AmtPercent", ResourceType = typeof(Localization.Resources))]
        public FixedContPecntType AmtPercent { get; set; }
    }
}