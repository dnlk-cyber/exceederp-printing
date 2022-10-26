using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class ProductCategory : TrackUserOperation
    {
        public int ProductCategoryId { get; set; }

        [Display(Name ="Category Name")]
        [Required]
        public string Description { get; set; }

        [Display(Name ="Inventory Category")]
        public string InventoryCategoryCode { get; set; }

        [Required]
        public int Index { get; set; }

        public string Parent { get; set; }
        [Required]
        public string SalesAccountCode { get; set; }
        public string InventoryAccountCode { get; set; }
        [Display(Name ="Unearned Account Code")]
        public string UnEarnedAccountCode { get; set; }

        public bool IsActive { get; set; }

        public string Remark { get; set; }
        [NotMapped]
        public bool HasTransaction { get; set; }
    }

}
