using ExceedERP.Core.Domain.HRM.SalaryStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
