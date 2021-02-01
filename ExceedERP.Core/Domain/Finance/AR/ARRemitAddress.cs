using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARRemitAddress : TrackUserOperation
    {
        public int ARRemitAddressID { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string Company { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "P.O.Box")]
        public string PostalCode { get; set; }
    }
}
