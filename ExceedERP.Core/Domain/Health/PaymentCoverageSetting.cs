using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Health
{
    public class PaymentCoverageSetting
    {
        public int PaymentCoverageSettingId { get; set; }
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        [Display(Name = "Payment Coverage(%)")]
        public decimal Payment { get; set; }
        public string Description { get; set; }
    }
}
