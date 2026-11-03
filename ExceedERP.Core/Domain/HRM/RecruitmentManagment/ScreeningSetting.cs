using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ScreeningSetting
    {
        public int ScreeningSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Vacancy))]
        public int VacancyId { get; set; }
        //[Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        //public GenderForVacancyGender Gender { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.OrganizationType))]
        public OrganizationType OrganizationType { get; set; }
        //[Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Experience))]
        //public int ExperienceYear { get; set; }
        //[Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Language))]
        //public Languages Language { get; set; }
        [Display(Name = "Based on Maximum Experience")]
        public bool IsBasedOnMaxExperience { get; set; }
        [Display(Name = "Apply Education Ranking")]
        public bool ApplyEducationRanking { get; set; }

        [Display(Name = "minimum Experience Percentage")]
        public decimal MiniExperiencePercentage { get; set; }
        [Display(Name = "Is  Percentage")]
        public bool IsPercentage { get; set; }



    }
}
