using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollContributionTypes
    {
        [Key]
        public int PayrollContributionTypesId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Contribution or Addition Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Contribution Account Code")]
        public string ContributionAccountCode { get; set; }
    }
}