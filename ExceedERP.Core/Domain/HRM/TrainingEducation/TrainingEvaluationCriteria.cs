using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingEvaluationCriteria
    {
        public int TrainingEvaluationCriteriaId { get; set; }
        public int TrainingEvaluationSettingCategoryId { get; set; }
        [Display(Name = "Rpt_Criteria", ResourceType = typeof(Localization.Resources))]
        public string Criteria { get; set; }
        public decimal Weight { get; set; }
        public string Result { get; set; }
    }
}
