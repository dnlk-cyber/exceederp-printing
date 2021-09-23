using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformanceAction
    {
        [Key]
        public int EmpActionId { get; set; }
        public int MeasurementDefinitionId { get; set; }
        //  public virtual MeasurementDefinition MeasurementDefinition { get; set; }
        public int MeasurementId { get; set; }
        // public virtual Measurement Measurement { get; set; }
        public int ActionId { get; set; }
        public virtual PerformanceAction PerformanceAction { get; set; }
        [Required]

        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Percentage of Weight")]
        public decimal EmpPerfPercentageofWeight { get; set; }
      
        [Display(Name = "Result")]
        public decimal Result { get; set; }
        [Display(Name = "Period")]
        public int QuarterId { get; set; }
        public virtual Quarter Quarter { get; set; }
        public string Remark { get; set; }
    }
}