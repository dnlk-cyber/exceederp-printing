using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Delegation : TrackUserOperation
    {
        [Key]
        public int DelegationId { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_Delegation_FromEmployee", ResourceType = typeof(Localization.Resources))]
        public int FromEmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_Attendance_Attendance_FromDateTime", ResourceType = typeof(Localization.Resources))]
        public DateTime? FromDateTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_FromDateTime", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string FromDateTimeAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //[Required]
        [Display(Name = "HR_Attendance_Attendance_ToDateTime", ResourceType = typeof(Localization.Resources))]
        public DateTime? ToDateTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_ToDateTime", ResourceType = typeof(Localization.Resources))]
        public string ToDateTimeAm { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_Delegation_ToEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        //[Required]
        //[Display(Name = "HR_Payroll_EmployeePayroll_Advance_PeriodId", ResourceType = typeof(Localization.Resources))]
        //public int GLPeriodId { get; set; }
        //public virtual GLPeriod Period { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public bool IsOpen { get; set; }
    }
}