using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Health
{
    public class HealthApprovalSetting
    {
        [Key]
        public int HealthApprovalSettingId { get; set; }

        // Indicates whether HR approval is required before sending health requests to payment
        [Display(Name = "Requires Hr approval for payment request")]
        public bool RequiresHrApprovalForPaymentRequest { get; set; }

        public string Description { get; set; }
    }


}
