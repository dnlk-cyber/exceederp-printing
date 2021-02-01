using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARSetupProductServiceAccount : TrackUserOperation
    {
        public int ARSetupProductServiceAccountID { get; set; }
        [Required]
        [Display(Name = "Product or Service")]
        public string Product { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        public virtual ICollection<ArProductService> ArProductServices { get; set; }
    }

    public class ArProductService : TrackUserOperation
    {
        public int ArProductServiceId { get; set; }
        public int ARSetupProductServiceAccountID { get; set; }
        public virtual ARSetupProductServiceAccount ArSetupProductService { get; set; }
        
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Item or Service")]
        public string ItemName { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        public string Remark { get; set; }
    }
}
