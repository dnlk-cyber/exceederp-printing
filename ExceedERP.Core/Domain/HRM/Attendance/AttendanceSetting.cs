////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\AttendanceSetting.cs
//
// summary:	Implements the attendance Setting 
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>    An attendance Setting . </summary>
    ///
    /// <remarks>    Kibrom Abera, 26/06/2025. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DailyAttendanceSetting : TrackUserOperation
    {
        [Key]
        public int DailyAttendanceSettingId { get; set; }

        [Display(Name = "Allow  Daily Attendance and Approval Only")]
        public bool IsDailyAttendanceAndApprovalAllowed { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }



}
