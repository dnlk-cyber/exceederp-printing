using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public class SupplierCategory : TrackUserOperation
    {
        public int SupplierCategoryID { get; set; }
        [Required]
        [Display(Name = "Supplier Category")]
        public string Category { get; set; }
        public string Description { get; set; }
    }
}