using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary>   A leave. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Leave:TrackUserOperation  //About Le
    {
        [Key ]
        public int TakenLeaveId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_LeaveType", ResourceType = typeof(Localization.Resources))]
        public LeaveType LeaveType { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Amount", ResourceType = typeof(Localization.Resources))]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
       
        [Display(Name = "HR_LeaveManagment_Leave_OrgName", ResourceType = typeof(Localization.Resources))]
        public string OrgName { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_Before", ResourceType = typeof(Localization.Resources))]
        public string Before { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_After", ResourceType = typeof(Localization.Resources))]
        public string After { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Enum_Leave_Day", ResourceType = typeof(Localization.Resources))]
        public LeaveDay Day { get; set; }
        [Display(Name = "MorningOrAfternoon", ResourceType = typeof(Localization.Resources))]
        public MorningOrAfternoon MorningOrAfternoon { get; set; }
       
        [Display(Name = "HR_LeaveManagment_Leave_Hour", ResourceType = typeof(Localization.Resources))]
        public int Hours { get; set; }
        public int? RequestId { get; set; }
        public LeaveActionTypes LeaveActionTypes { get; set; }
    }
}