using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingEvaluationCriteria
    {
        public int TrainingEvaluationCriteriaId { get; set; }
        public int TrainingEvaluationSettingCategoryId { get; set; }
        public string Criteria { get; set; }
    }
}
