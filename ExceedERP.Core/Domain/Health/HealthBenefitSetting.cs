using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Health
{
    public class HealthBenefitSetting
    {
        public int HealthBenefitSettingId { get; set; }
        public HealthBenefitType BenefitType { get; set; }
        public decimal? BenfitValue { get; set; }
        public string Description { get; set; }
    }
    public enum HealthBenefitType
    {
        NA,
        [Display(Name = "Eye-glass")]
        EyeGlass,
        Maternity,
        Accident,
        Family

    }
}
