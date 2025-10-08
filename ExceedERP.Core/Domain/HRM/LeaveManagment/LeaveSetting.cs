using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary>   A  leave Type settings. </summary>
    ///
    /// <remarks>   Kibrom Abera, Feb 7,2025. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    public class LeaveTypeSetting : TrackUserOperation
    {
        public int LeaveTypeSettingId { get; set; }
        [Display(Name = "Leave Type")]
        public LeaveType LeaveType { get; set; }
        [Display(Name = "Is Working Days")]
        public bool IsWorkingDays { get; set; }
        [Display(Name = "Is Consecutive Days")]
        public bool IsConsecutiveDays { get; set; }
        [Display(Name = "Maximum Amount")]
        public decimal MaximumAmount { get; set; }
        [Display(Name = "Block Back-Dated Leave Requests")]
        public bool BlockBackdatedLeave { get; set; }
        [Display(Name = "Max Days Back")]
        public int? MaxDaysBack { get; set; }  // null = no restriction
        public string Remark { get; set; }


    }

}
