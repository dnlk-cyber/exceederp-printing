using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class EmpPayrollNoDaysWorked:TrackUserOperation 
    {
        [Key]
        public int EmpPayrollNoDaysWorkedId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        public GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_Days", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Days { get; set; }
        [Display(Name = "HR_Attendance_Absence_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }

        [Display(Name = "HR_Attendance_Absence_MeasureTaken", ResourceType = typeof(Localization.Resources))]
        public string MeasureTaken { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}