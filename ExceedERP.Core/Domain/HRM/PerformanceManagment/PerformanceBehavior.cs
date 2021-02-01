using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceBehavior
    {
        [Key]
        public int PerformanceBehaviorId { get; set; }
        [Display  (Name="Behavior Measure")]
        public string MeasureId { get; set; }
        [Display (Name="Percentage Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Performance Data source")]
        public string DataSource { get; set; }
        public string Remark { get; set; }


    }
}