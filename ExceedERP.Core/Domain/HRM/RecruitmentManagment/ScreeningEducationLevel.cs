using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ScreeningEducationLevel
    {
        public int ScreeningEducationLevelId { get; set; }
        public int ScreeingSettingId { get; set; }
        [Display(Name = "Education Level")]
        public int EducationLevelId { get; set; }
        public string Remark { get; set; }
    }
}
