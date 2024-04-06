using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class InternalApplicantScreeningResult
    {
        public int InternalApplicantScreeningResultId { get; set; }
        public int ApplicationFormId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Vacancy))]
        public int VacancyId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}
