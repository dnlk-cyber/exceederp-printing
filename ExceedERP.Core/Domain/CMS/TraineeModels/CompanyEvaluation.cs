using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
