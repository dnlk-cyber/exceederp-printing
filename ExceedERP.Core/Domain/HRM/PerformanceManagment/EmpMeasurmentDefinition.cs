using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
   public class EmpMeasurmentDefinition
    {
        public int EmpMeasurmentDefinitionId { get; set; }
        public int EmployeePerformanceId { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_EvaluationCriteria", ResourceType = typeof(Localization.Resources))]
        public string EvaluationCriteria { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_EvaluationType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_PercentageofWeight", ResourceType = typeof(Localization.Resources))]
        public decimal DefnPercentageofWeight { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_Result", ResourceType = typeof(Localization.Resources))]
        public decimal Result { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_Role", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel Role { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int OrgStuructureId { get; set; }
        [NotMapped]
        public int CheckEmp { get; set; }
    }
}
