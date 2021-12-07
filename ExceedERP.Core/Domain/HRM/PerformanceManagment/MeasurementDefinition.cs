using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class MeasurementDefinition:TrackUserOperation 
    {
        [Key]
        public int MeasurementDefinitionId { get; set; }
        [Required ]
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_EvaluationCriteria", ResourceType = typeof(Localization.Resources))]
        public string EvaluationCriteria { get; set; }
        [Required]
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_EvaluationType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_PercentageofWeight", ResourceType = typeof(Localization.Resources))]
        public decimal DefnPercentageofWeight { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_Result", ResourceType = typeof(Localization.Resources))]
        public decimal Result { get; set; }
      
        public int EmployeeId { get; set; }
        [Display(Name = "HR_PerformanceManagment_MeasurementDefinition_Role", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel Role { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int OrgStuructureId { get; set; }
        public bool IsDepartmentLevel { get; set; }
        [NotMapped]
        public int CheckEmp { get; set; }
    }
}