using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common
{
    public class VoucherDistribution : TrackUserOperation
    {
       // [Required] do later
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }

        [Display(Name = "Additional Description")]
        public string AdditionalDescription { get; set; }
        [Range(0.0, 999999999, ErrorMessage = "Debit can't be negative value")]
        public decimal Debit { get; set; }
        [Range(0.0, 999999999, ErrorMessage = "Credit can't be negative value")]
        public decimal Credit { get; set; }
    }
}
