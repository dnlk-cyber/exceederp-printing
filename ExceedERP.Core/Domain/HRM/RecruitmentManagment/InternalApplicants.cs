using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class InternalApplicants
    {
        public int InternalApplicantsId { get; set; }
        public int ApplicationFormId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Employee))]
        public int? EmployeeId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ApplicantName))]
        public string ApplicantName { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public Gender Gender { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Age))]
        public int? Age { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Address))]
        public string Address { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Training))]
        public string Training { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.VacancyCode))]
        public string VacancyCode { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Result))]
        [NotMapped]
        public double Result { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Description))]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Language))]
        public Languages Language { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.UploadFile))]
        public string FilePath { get; set; }
        [NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.FileName))]
        public string FileName { get; set; }
    }
}
