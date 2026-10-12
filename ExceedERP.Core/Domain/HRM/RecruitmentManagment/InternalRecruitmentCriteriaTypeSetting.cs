using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class InternalRecruitmentCriteriaTypeSetting: TrackUserSettingOperation
    {
        public int InternalRecruitmentCriteriaTypeSettingId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ExamSource))]
        public RecruitmentSource ExamSource { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ExamType))]
        public ExamType ExamType { get; set; }
        public int Squence { get; set; }
        [Display(Name ="Is selected")]
        public bool IsPercent { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.SettingValue))]
        public decimal SettingVlaue { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        public decimal Rate { get; set; }
    }
}
