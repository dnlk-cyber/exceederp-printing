using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceAction
    {
        [Key]
        public int ActionId { get; set; }

        public int? MeasurementDefinitionId { get; set; }
        public int? MeasurementId { get; set; }
        [Display(Name = "HR_PerformanceManagment_PerformanceAction_ActionName", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string ActionName { get; set; }
        [Required]
        [Display(Name = "HR_PerformanceManagment_PerformanceAction_PercentageofWeight", ResourceType = typeof(Localization.Resources))]
        public decimal AcPercentageofWeight { get; set; }
        [Display(Name = "HR_PerformanceManagment_PerformanceAction_Result", ResourceType = typeof(Localization.Resources))]
        public decimal Result { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}