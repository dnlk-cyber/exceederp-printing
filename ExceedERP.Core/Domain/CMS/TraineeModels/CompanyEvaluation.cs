using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public class CompanyEvaluation : TrackUserSettingOperation
    {
        [Key]
        public int CompanyEvaluationId { get; set; }
        public int CmsTraineeId { get; set; }

        public virtual CmsTrainee Trainee { get; set; }

        [Display(Name = "ISO Checklist")]
        public int ISOChecklistId { get; set; }

        public string Value { get; set; }
        public string Remark { get; set; }
    }
}
