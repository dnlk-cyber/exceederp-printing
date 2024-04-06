using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class RetirementSetting
    {
        public int RetirementSettingId { get; set; }
        [Display(Name = "RetireSetting", ResourceType = typeof(Localization.Resources))]
        public RetireSetting RetireSetting { get; set; }
        [Display(Name = "CurrentValue", ResourceType = typeof(Localization.Resources))]
        public double CurrentValue { get; set; }
    }
}
