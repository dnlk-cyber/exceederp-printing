using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformanceScore
    {
        public int EmployeePerformanceScoreId { get; set; }
        public int? MeasurementDefinitionId { get; set; }
        public int? MeasurementId { get; set; }
        public int ActionId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Percentage of Weight")]
        public decimal EmpPerfPercentageofWeight { get; set; }
        [Display(Name = "Result")]
        public decimal Result { get; set; }
        [Display(Name = "Period")]
        public int QuarterId { get; set; }
        public int FiscalYearId { get; set; }
        public bool IsDepartmentLevel { get; set; }
        public bool IsApproved { get; set; }
        public string Remark { get; set; }
    }
}
