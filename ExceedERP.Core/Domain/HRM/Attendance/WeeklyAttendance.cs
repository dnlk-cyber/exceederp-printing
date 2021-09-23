using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class WeeklyAttendance
    {
      
        [Key]
        public int WeeklyAttendanceId { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_Attendance_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DayOne", ResourceType = typeof(Localization.Resources))]
        public bool DayOne { get; set; }
        public DateTime? DayOneDate { get; set; }
        public string DayOneAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DayTwo", ResourceType = typeof(Localization.Resources))]
        public bool DayTwo { get; set; }
        public DateTime? DayTwoDate { get; set; }
        public string DayTwoAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DayThree", ResourceType = typeof(Localization.Resources))]
        public bool DayThree { get; set; }
        public DateTime? DayThreeDate { get; set; }
        public string DayThreeAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DayFour", ResourceType = typeof(Localization.Resources))]
        public bool DayFour { get; set; }

        public DateTime? DayFourDate { get; set; }
        public string DayFourAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DayFive", ResourceType = typeof(Localization.Resources))]
        public bool DayFive { get; set; }
        public DateTime? DayFiveDate { get; set; }
        public string DayFiveAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DaySix", ResourceType = typeof(Localization.Resources))]
        public bool DaySix { get; set; }
        public DateTime? DaySixDate { get; set; }
        public string DaySixAmharicDate { get; set; }
        [Display(Name = "HR_Attendance_WeeklyAttendance_DaySeven", ResourceType = typeof(Localization.Resources))]
        public bool DaySeven { get; set; }
        public DateTime? DaySevenDate { get; set; }
        public string DaySevenAmharicDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

        //////////////////////////////////////////////
        public bool DayOneAN { get; set; }
        public bool DayTwoAN { get; set; }
        public bool DayThreeAN { get; set; }
        public bool DayFourAN { get; set; }
        public bool DayFiveAN { get; set; }
        public bool DaySixAN { get; set; }
        public bool DaySevenAN { get; set; }

        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
        public int Department { get; set; }
    }
}