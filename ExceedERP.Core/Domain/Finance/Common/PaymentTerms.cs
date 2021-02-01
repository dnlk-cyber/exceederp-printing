using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.Common
{
    public class PaymentTerms : TrackUserOperation
    {
        public int PaymentTermsID { get; set; }
        [Required]
        [Display(Name = "Payment Term in Days")]
        public int PaymentTermsDays { get; set; }
        public string Description { get; set; }
    }
}
