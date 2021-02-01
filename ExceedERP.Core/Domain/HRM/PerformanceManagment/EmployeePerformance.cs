using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformance
    {
        [Key]
        public int EmployeePerformanceId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Result")]
        public decimal Result { get; set; }
        [NotMapped]
        public string Grade { get; set; }
        [NotMapped]
        public bool IsApproved { get; set; }
        public string Remark { get; set; }

    }
}