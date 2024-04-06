using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantDetail
    {
        public int ApplicantDetailId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        [Display(Name = "Gender", ResourceType = typeof(Localization.Resources))]
        public Gender Sex { get; set; }
        [Display(Name = "Age", ResourceType = typeof(Localization.Resources))]
        public int Age { get; set; }
        [Display(Name = "ApplicantName", ResourceType = typeof(Localization.Resources))]
        public string ApplicantName { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Localization.Resources))]
        public string Address { get; set; }
        [Display(Name = "Training", ResourceType = typeof(Localization.Resources))]
        public string Training { get; set; }
        [Display(Name = "VacancyCode", ResourceType = typeof(Localization.Resources))]
        public string VacancyCode { get; set; }
        [Display(Name = "Result", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public double Result { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Language", ResourceType = typeof(Localization.Resources))]
        public Languages Language { get; set; }
      //  [Display(Name ="Language")]
       // public string LanguageForDisplay { get; set; }
        [DisplayName("UploadFile")]
        public string FilePath { get; set; }
        [NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        [Display(Name = "FileName", ResourceType = typeof(Localization.Resources))]
        public string FileName { get; set; }

    }
}
