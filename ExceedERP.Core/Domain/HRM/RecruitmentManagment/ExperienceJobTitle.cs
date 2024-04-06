using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ExperienceJobTitle
    {
        public int ExperienceJobTitleId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.JobTitle))]
        public string JobTitle { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}
