using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollCostCenter
    {
        [Key]
        public int PayrollCostCenterId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Values")]
        public string Values { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}