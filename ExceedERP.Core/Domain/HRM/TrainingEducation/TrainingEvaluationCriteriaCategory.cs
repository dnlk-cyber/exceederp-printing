using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingEvaluationCriteriaCategory
    {
        public int TrainingEvaluationSettingCategoryId { get; set; }
        [Display(Name = "GLPeriod_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        //public EvaluationType EvaluationType { get; set; }
    }
    //public enum EvaluationType
    //{
    //    TrainingEvaluation,
    //    EmployeeGaPEvaluation

    //}

}
