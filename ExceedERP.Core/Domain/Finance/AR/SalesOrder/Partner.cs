using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class Partner : TrackUserOperation
    {
        public int PartnerId { get; set; }
        [Display(Name = "Code")]
        public string PartnerCode { get; set; }//sub account
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
       
        [Display(Name = "Partner Type")]
        public int PartnerTypeId { get; set; }
        [Required]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
}
