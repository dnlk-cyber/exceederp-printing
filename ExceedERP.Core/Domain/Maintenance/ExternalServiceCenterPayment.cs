using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ExternalServiceCenterPayment
    {
        [Key]
        public int ExternalServiceCenterPaymentId { get; set; }
        [Display(Name = "Cost Type")]
        public string CostType { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub-Total")]
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        [Required]
        public int GlTaxRateId { get; set; }
        public decimal Total { get; set; }
        public string Invoice { get; set; }
        public Guid ServiceCenterJobId { get; set; }
    }
}
