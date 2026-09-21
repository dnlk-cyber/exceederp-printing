using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeavePayableSetting: TrackUserOperation
    {
        [Key]
        public int LeavePayableSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_Attendance_EmpPayrollNoDaysWorked_Days))]
        public decimal CurrentValue { get; set; }

        [Display(Name = "Include Current Fiscal Year Days")]
        public bool IncludeCurrentFiscalYearDays { get; set; }

    }
}
