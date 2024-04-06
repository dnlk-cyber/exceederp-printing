using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EvaluationCriteriaActionType
    {

        public int EvaluationCriteriaActionTypeId { get; set; }
        [Display(Name = "PerformanceCriteriaActionType", ResourceType = typeof(Localization.Resources))]
        public PerformanceCriteriaActionType PerformanceCriteriaActionType { get; set; }
        [Display(Name = "HR_Discipline_DisciplineMeasuerByMoney_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
    }
    public enum PerformanceCriteriaActionType
    {
        [Display(Name = "Enum_PerformanceCriteriaActionType_Open", ResourceType = typeof(Localization.Resources))]
        Open,
        [Display(Name = "Enum_PerformanceCriteriaActionType_Open_Sum_Of_Actions", ResourceType = typeof(Localization.Resources))]
        Sum_Of_Actions
    }
}
