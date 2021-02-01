////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\plan.cs
//
// summary:	Implements the plan class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.HRMPlan
{
   ////////////////////////////////////////////////////////////////////////////////////////////////////
   /// <summary>    A plan. </summary>
   ///
   /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
   ////////////////////////////////////////////////////////////////////////////////////////////////////

 

    public class LeavePlan
        :TrackUserOperation
    {
        [Key ]
        public int LeavePlanId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
       [Display(Name = "HR_HRMPlan_LeavePlan_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_LeavePlan_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required ]
        [Display(Name = "HR_HRMPlan_LeavePlan_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_LeavePlan_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HRMPlan_LeavePlan_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_LeavePlan_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Required]
        [Display(Name = "HR_HRMPlan_LeavePlan_LeaveAmount", ResourceType = typeof(Localization.Resources))]
        public decimal LeaveAmount { get; set; }
          [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
