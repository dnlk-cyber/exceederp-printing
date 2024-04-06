using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformanceScore
    {
        [Key]
        public int EmployeePerformanceScoreId { get; set; }
        public int EmployeeMeasurementId { get; set; }
        public int PerformanceActionId { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
       // public bool IsChecked { get; set; }
        public string Comment { get; set; }
        public string Remark { get; set; }
    }
}
