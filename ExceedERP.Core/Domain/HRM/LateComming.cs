using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class LateComming
    {
        [Key]
        public int LateCommingId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_LateComming_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Attendance_LateComming_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_Attendance_LateComming_Date", ResourceType = typeof(Localization.Resources))]
        public string DateLateAmharic { get; set; }
        [Display(Name = "HR_Attendance_LateComming_Time", ResourceType = typeof(Localization.Resources))]
        public int Time { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }

    public class LateComingSummary
    {
        [Key]
        public int LateComingSummaryId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Person_Employee { get; set; }
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_Period", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }

        public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_LateComingSummary_TotalHours", ResourceType = typeof(Localization.Resources))]
        public decimal  TotalHours { get; set; }
        [Display(Name = "HR_LateComingSummary_TotalDays", ResourceType = typeof(Localization.Resources))]
        public decimal TotalDays { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public  bool Check { get; set; }
        public bool Processed { get; set; }
    }
}