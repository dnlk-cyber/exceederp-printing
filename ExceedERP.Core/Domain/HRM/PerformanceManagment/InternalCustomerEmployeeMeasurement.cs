using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class InternalCustomerEmployeeMeasurement
    {
        [Key]
        public int InternalCustomerEmployeeMeasurementId { get; set; }
        public int InternalCustomerEmployeePerformanceId { get; set; }
        public int MeasurmentId { get; set; }
        public decimal Weight { get; set; }
        public decimal Result { get; set; }
        
        public string Remark { get; set; }


    }
}