using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class InternalApplicantEducation
    {
        [Key]
        public int InternalApplicantEducationId { get; set; }
        public int ApplicationFormId { get; set; }
        public int InternalApplicantsId { get; set; }
        [Display(Name = "Education Level")]
        public int EducationLevelSettingId { get; set; }
        [Display(Name = "Education Field")]
        public int EducationFieldSettingId { get; set; }
        [Display(Name = "Institution")]
        public string EducationInstitution { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}
