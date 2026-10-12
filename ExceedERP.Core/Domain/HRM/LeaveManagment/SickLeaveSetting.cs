using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary>   A Sick leave settings. </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/31/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SickLeaveSetting
    {
        public int SickLeaveSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.CurrentValue))]
        public double CurrentValue { get; set; }
    }
    public class SickLeaveSalarySetting: TrackUserOperation
    {
        public int SickLeaveSalarySettingId { get; set; }
        [Display(Name = "Type")]
        public MedicalLeaveType? Type { get; set; }

        [Display(Name = "From Days")]
        public int FromDays { get; set; }
        [Display(Name = "To Days")]
        public int ToDays { get; set; }
        [Display(Name = "Payable Salary in %")]
        [Range(0, 100, ErrorMessage = "Payable Salary in % must be between 0 and 100.")]
        public decimal PayableSalaryInPercent { get; set; }
        [Display(Name = "Is Terminated")]
        public bool IsTerminated { get; set; }

    }
    public enum MedicalLeaveType
    {
        Illness = 1,
        Accident = 2,
        Others = 2
    }

}
