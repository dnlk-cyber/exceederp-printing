using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelOrderAdvancePayment : TrackUserOperation
    {
        public int ApTravelOrderAdvancePaymentId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }
        [Required]
        public TravelPaymentType Type { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "UoM")]
        public string UoM { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

    }
}
