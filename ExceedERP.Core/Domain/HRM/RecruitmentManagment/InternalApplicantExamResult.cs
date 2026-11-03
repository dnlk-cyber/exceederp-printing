using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Display(Name ="Actual Value")]
        public decimal ActualValue { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.CurrentValue))]
        public decimal CurrentResult { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Rpt_Total))]
        public decimal Total { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark { get; set; }
        [NotMapped]
        public bool IsInterview { get; set; }
    }
}
