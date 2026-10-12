using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class ApplicantScreeningResultAttachment
    {
        public int ApplicantScreeningResultAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public bool IsInternal { get; set; }
        public int? InternalApplicantScreeningResultId { get; set; }
        public int? ExternalApplicantScreeningResultId { get; set; }

    }



}
