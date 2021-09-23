using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class HealthRankAndValueSetting
    {
        public int HealthRankAndValueSettingId { get; set; }
        public int RankId { get; set; }
        public Rank Rank { get; set; }
        public string CurrentValue { get; set; }
    }
}
