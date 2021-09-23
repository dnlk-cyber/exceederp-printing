////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\attendance.cs
//
// summary:	Implements the attendance class
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>    An attendance. </summary>
    ///
    /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Attendance
    {
     [Key ]
       public int AttendanceId { get; set; }
     [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
     public string EmployeeCode { get; set; }
     [Required]
     [Display(Name = "HR_Attendance_Attendance_PersonEmployee", ResourceType = typeof(Localization.Resources))]
      public int EmployeeId { get; set; }
     public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_Attendance_AttendanceType", ResourceType = typeof(Localization.Resources))]
       public int AttendanceType { get; set; }
        [DataType(DataType.DateTime)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
       [Required ]
       [Display(Name = "HR_Attendance_Attendance_FromDateTime", ResourceType = typeof(Localization.Resources))]
       public DateTime FromDateTime { get; set; }
        [DataType(DataType.DateTime)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Attendance_Attendance_ToDateTime", ResourceType = typeof(Localization.Resources))]
       public DateTime ToDateTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_DelayTime", ResourceType = typeof(Localization.Resources))]
       public int DelayTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_NoOfAbcent", ResourceType = typeof(Localization.Resources))]
        public int NoOfAbcent { get; set; }
        [Display(Name = "HR_Attendance_Attendance_MeasureTaken", ResourceType = typeof(Localization.Resources))]
       public string MeasureTaken { get; set; }
       [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
       public string Remark { get; set; }

       }

    
}
