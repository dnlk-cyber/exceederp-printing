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

        // Some companies pay leave-in-lieu using Basic Salary + a separate Taxable Allowance
        // (see the "Payment in Lieu of Leave" form). Off by default so existing companies see
        // no change to the grid or the calculation.
        [Display(Name = "Enable Taxable Allowance")]
        public bool EnableTaxableAllowance { get; set; }

    }
}
