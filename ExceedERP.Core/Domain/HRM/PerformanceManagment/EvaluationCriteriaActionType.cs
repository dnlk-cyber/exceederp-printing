using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EvaluationCriteriaActionType
    {
        public int EvaluationCriteriaActionTypeId { get; set; }
        public PerformanceCriteriaActionType PerformanceCriteriaActionType { get; set; }
        public bool IsActive { get; set; }
    }
    public enum PerformanceCriteriaActionType
    {
        Open,
        Sum_Of_Actions
    }
}
