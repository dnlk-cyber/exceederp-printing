using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArCustomerProfile : TrackUserOperation
    {
        public int ArCustomerProfileId { get; set; }
        [Required]
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }
        [Required]
        [Display(Name = " Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = " TIN Number")]
        [StringLength(10, MinimumLength = 10)]
        public string TinNo { get; set; }
        [Required]
        [Display(Name = "Customer Type")]
        public string CustomerType { get; set; }
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
        public virtual ICollection<ArInvoiceViewModel> ArInvoiceViewModel { get; set; }
    }
}
