using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ExternalSalary : TrackUserOperation
    {
        [Key]
        public int ExternalSalaryId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee  { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal Salary { get; set; }
    }
}