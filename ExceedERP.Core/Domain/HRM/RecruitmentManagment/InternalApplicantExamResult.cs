using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class InternalApplicantExamResult
    {
        public int InternalApplicantExamResultId { get; set; }
        public int InternalApplicantScreeningId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ExamType))]
        public ExamType ExamType { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.SettingValue))]
        public decimal SettingValue { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.CurrentValue))]
        public decimal CurrentResult { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Rpt_Total))]
        public decimal Total { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}
