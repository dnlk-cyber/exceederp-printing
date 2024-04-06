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
        [Display(Name ="Evalution Type")]
        public EvaluationType EvalutionType { get; set; }
        public bool IsDepartmentLevel { get; set; }
        [NotMapped]
        public int CheckEmp { get; set; }
        [Display(Name= "Criteria Category")]
        public int CriteriaCategoryId { get; set; }
        public bool IsMainCompanyLevel { get; set; }
    }
    
    public class MainCompanyEvalutionSetting
    {
        public int MainCompanyEvalutionSettingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        [Display(Name = "Enum_LookUpTypes_OLD_JOB_TITLE", ResourceType = typeof(Localization.Resources))]
        public int? LookUpId { get; set; }
        public int MeasurementDefinitionId { get; set; }
        public virtual MeasurementDefinition Measurement { get; set; }

    }
}