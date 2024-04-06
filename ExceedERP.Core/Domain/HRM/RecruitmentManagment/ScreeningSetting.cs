using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ScreeningSetting
    {
        public int ScreeningSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Vacancy))]
        public int VacancyId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public GenderForVacancyGender Gender { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.OrganizationType))]
        public OrganizationType OrganizationType { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Experience))]
        public int ExperienceYear { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Language))]
        public Languages Language { get; set; }
    }
}
