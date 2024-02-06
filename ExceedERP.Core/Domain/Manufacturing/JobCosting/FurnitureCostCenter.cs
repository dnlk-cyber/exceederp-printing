using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
    public class FurnitureCostCenter
    {
        public int FurnitureCostCenterId { get; set; }
        [Required]
        [Display(Name = "Cost Center Name")]
        public string CostCenterName { get; set; }
        [Display(Name = "Account Number")]
        public string Description { get; set; }
    }
}
