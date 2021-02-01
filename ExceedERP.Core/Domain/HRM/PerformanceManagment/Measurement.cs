using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class Measurement
    {
        [Key]
        public int MeasurementId { get; set; }
        public int MeasurementDefinitionId { get; set; }
        //  public virtual MeasurementDefinition MeasurementDefinition { get; set; }
        [Required ]
        [Display(Name = "HR_PerformanceManagment_Measurement_MeasurementName", ResourceType = typeof(Localization.Resources))]
        public string MeasurementName { get; set; }
        [Required ]
        [Display(Name = "HR_PerformanceManagment_Measurement_PercentageofWeight", ResourceType = typeof(Localization.Resources))]
        public decimal MesPercentageofWeight { get; set; }
        public decimal Result { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}