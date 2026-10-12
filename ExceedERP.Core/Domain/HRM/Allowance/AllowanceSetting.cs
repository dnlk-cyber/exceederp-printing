////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\allowance.cs
//
// summary:	Implements the allowance class
////////////////////////////////////////////////////////////////////////////////////////////////////

using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class AllowanceSetting: TrackUserOperation
    {
        public int AllowanceSettingId { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowanceDefination_AllowanceType", ResourceType = typeof(Localization.Resources))]
        public AllowanceType AllowanceType { get; set; }
        [Display(Name = "Is only for Head")]
        public bool IsOnlyForHead { get; set; }
        [Display(Name = "Job title ")]
        public int? LookupId { get; set; }
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
