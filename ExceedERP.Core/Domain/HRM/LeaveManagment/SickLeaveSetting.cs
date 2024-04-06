using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class SickLeaveSetting
    {
        public int SickLeaveSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.CurrentValue))]
        public double CurrentValue { get; set; }
    }
}
