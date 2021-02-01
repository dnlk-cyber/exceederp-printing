using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PettyCash
{
    public class ApPettyCashPayment : TrackUserOperation
    {
        public int ApPettyCashPaymentId { get; set; }
        public int ApPettyCashId { get; set; }
        public virtual ApPettyCash PettyCash { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }

       
    }
}
