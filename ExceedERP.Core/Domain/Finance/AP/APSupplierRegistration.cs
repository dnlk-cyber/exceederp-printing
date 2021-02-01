using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
//using ExceedERP.Core.Domain.Procurement.PurchaseOrder;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class APSupplierRegistration : TrackUserOperation
    {
        public int APSupplierRegistrationID { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Required]
        public string Account { get; set; }
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "TIN Number")]
        [StringLength(10, MinimumLength = 10)]
        public string TINNo { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        public string Country { get; set; }
        [Display(Name = "Supplier Type")]
        public string SupplierType { get; set; }
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<APsupplierBlacklist> APsupplierBlacklist { get; set; }
    }
}
