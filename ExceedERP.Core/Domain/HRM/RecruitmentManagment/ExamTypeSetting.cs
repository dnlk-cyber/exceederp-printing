using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ExamTypeSetting
    {
        public int ExamTypeSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ExamSource))]
        public RecruitmentSource ExamSource { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ExamType))]
        public ExamType ExamType { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.SettingValue))]
        public decimal SettingVlaue { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
    }
}
