using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantScreenResult
    {
        [Key]
        public int ApplicantScreenResultId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Vacancy))]
        public int VacancyId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.JobTitle))]
        [NotMapped]
        public string JobTitle { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Location))]
        [NotMapped]
        public string Location { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}