using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeeMeasurementDefinition:TrackUserOperation 
    {
        [Key]
        public int EmpMeasurementDefinitionId { get; set; }
        [Required ]
        public int MeasurementDefinitionId { get; set; }
        public virtual MeasurementDefinition MeasurementDefinition { get; set; }
        [Required]
        [Display(Name = "HR_PerformanceManagment_EmployeeMeasurementDefinition_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_PerformanceManagment_EmployeeMeasurementDefinition_PercentageofWeight", ResourceType = typeof(Localization.Resources))]
        public decimal EmpPercentageofWeight { get; set; }

        [Display(Name = "HR_PerformanceManagment_EmployeeMeasurementDefinition_Result", ResourceType = typeof(Localization.Resources))]
        public decimal Result { get; set; }
        [Display(Name = "HR_PerformanceManagment_EmployeeMeasurementDefinition_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int QuarterId { get; set; }
        public virtual Quarter Quarter{ get; set; }
        ////[NotMapped ]
        //  [Display(Name = "HR_PerformanceManagment_EmployeeMeasurementDefinition_Role", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel Role { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }

    }
}