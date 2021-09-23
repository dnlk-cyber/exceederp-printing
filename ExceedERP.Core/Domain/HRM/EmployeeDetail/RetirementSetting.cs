using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class RetirementSetting
    {
        public int RetirementSettingId { get; set; }
        public RetireSetting RetireSetting { get; set; }
        public double CurrentValue { get; set; }
    }
}
