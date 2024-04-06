using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ScreeningExperienceJobtitle
    {
        public int ScreeningExperienceJobtitleId { get; set; }
        public int ScreeingSettingId { get; set; }
        [Display(Name = "Education Field")]
        public int ExperienceJobTitleId { get; set; }
        public string Remark { get; set; }
    }
}
