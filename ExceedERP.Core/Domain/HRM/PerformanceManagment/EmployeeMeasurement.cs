using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeeMeasurement
    {
        [Key]
        public int EmpMeasurementId { get; set; }
        public int EmployeePerformanceId { get; set; }
        [Display(Name = "Measurment")]
        public int MeasurmentId { get; set; }
        public decimal Weight { get; set; }
        public decimal Result { get; set; }
        
        public string Remark { get; set; }


    }
}