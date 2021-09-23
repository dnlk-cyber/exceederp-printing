using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class ProductCategory : TrackUserOperation
    {
        public int ProductCategoryId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Index { get; set; }

        public string Parent { get; set; }
        [Required]
        public string SalesAccountCode { get; set; }
        public string InventoryAccountCode { get; set; }
        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }

}
