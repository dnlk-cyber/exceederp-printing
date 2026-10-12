////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\attendance.cs
//
// summary:	Implements the attendance class
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>    An attendance Exception . </summary>
    ///
    /// <remarks>    Kibrom Abera, 01/02/2025. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AttendanceException: DepartmentOperations
    {
        [Key]
        public int AttendanceExceptionId { get; set; }
        [Required]
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_Attendance_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_From", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_From", ResourceType = typeof(Localization.Resources))]
        public string  DateAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_LeaveRequest_To", ResourceType = typeof(Localization.Resources))]
        public DateTime ToDate { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_To", ResourceType = typeof(Localization.Resources))]
        public string  ToDateAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }

    }


}
