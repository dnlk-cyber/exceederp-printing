using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.JobCosting
{
    public class CostCenter
    {
        public int CostCenterId { get; set; }
        [Required]
        [Display(Name = "Cost Center Name")]
        public string CostCenterName { get; set; }
        [Display(Name = "Account Number")]
        public string Description { get; set; }
    }
}
