using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeavePayableSetting
    {
        [Key]
        public int LeavePayableSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.CurrentValue))]
        public decimal CurrentValue { get; set; }
    }
}
