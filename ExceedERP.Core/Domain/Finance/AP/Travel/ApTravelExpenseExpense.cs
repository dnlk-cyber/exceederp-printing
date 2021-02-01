using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class ApTravelExpenseExpense : TrackUserOperation
    {
        public int ApTravelExpenseExpenseId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
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

       
    }
}
