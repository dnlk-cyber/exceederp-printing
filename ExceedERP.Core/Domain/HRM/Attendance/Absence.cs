using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class Absence
    {
        public int AbsenceId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_Absence_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Attendance_Absence_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_Attendance_Absence_From", ResourceType = typeof(Localization.Resources))]
        public string FromAbscAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Attendance_Absence_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_Attendance_Absence_To", ResourceType = typeof(Localization.Resources))]
        public string ToAbscAmharic { get; set; }
        [Display(Name = "HR_Attendance_Absence_NoOfWorkingDays", ResourceType = typeof(Localization.Resources))]
        public int NoOfWorkingDays { get; set; }
        [Display(Name = "HR_Attendance_Absence_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
      
        [Display(Name = "HR_Attendance_Absence_MeasureTaken", ResourceType = typeof(Localization.Resources))]
        public string MeasureTaken { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}