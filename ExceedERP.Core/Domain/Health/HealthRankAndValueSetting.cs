using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Health
{
    public class HealthRankAndValueSetting
    {
        public int HealthRankAndValueSettingId { get; set; }
        public int RankId { get; set; }
        public Rank Rank { get; set; }
        public string CurrentValue { get; set; }
    }
    public class HealthResponsiblityLevelValueSetting
    {
        [Key]
        public int HealthResponsiblityLevelValueSettingId { get; set; }
        [Display(Name = "Responsibility Level")]
        public ResponsibilityLevel Level { get; set; }
        public string CurrentValue { get; set; }
    }
}
