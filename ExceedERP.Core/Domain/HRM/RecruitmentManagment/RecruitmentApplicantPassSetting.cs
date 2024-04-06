using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class RecruitmentApplicantPassSetting
    {
        public int RecruitmentApplicantPassSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.TotalValue))]
        public double TotalValue { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.PassScore))]
        public double PassScore { get; set; }
        [Display(Name = "RecruitmentApplicantPassSetting_Vacancy", ResourceType = typeof(Localization.Resources))]
        public int VacancyId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}
