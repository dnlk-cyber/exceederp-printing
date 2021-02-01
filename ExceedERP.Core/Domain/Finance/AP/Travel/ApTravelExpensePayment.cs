using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{

    public class ApTravelExpensePayment : TrackUserOperation
    {
        public int ApTravelExpensePaymentId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Reference { get; set; }

    }
}
