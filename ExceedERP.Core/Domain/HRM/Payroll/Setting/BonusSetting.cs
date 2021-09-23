using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Payroll
{
    public class BonusSetting
    {
        public int BonusSettingId { get; set; }
        public BonusType BunusType { get; set; }
        public string Multiplier { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int? GlFiscalYearId { get; set; }
        public decimal CurrentValue { get; set; }
        public string Remark { get; set; }
    }
}
