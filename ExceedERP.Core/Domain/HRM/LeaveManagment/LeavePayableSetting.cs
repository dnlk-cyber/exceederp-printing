using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeavePayableSetting
    {
        [Key]
        public int LeavePayableSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_Attendance_EmpPayrollNoDaysWorked_Days))]
        public decimal CurrentValue { get; set; }
    }
}
