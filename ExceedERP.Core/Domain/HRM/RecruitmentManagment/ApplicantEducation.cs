using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantEducation
    {
        [Key]
        public int ApplicantEducationId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        public int ApplicantDetailId { get; set; }
        [Display(Name = "EducationLevel")]
        public int EducationLevelSettingId { get; set; }
        [Display(Name = "EducationField")]
        public int EducationFieldSettingId { get; set; }
        [Display(Name = "Institution")]
        public string EducationInstitution { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}