using System;
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
        [Display(Name = "Institution Name")]
        public string EducationInstitution { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Graduation Date")]
        public DateTime? GraduationDate { get; set; }
        public decimal? CGPA { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}