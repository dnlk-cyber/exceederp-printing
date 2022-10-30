using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class Product : Operations
    {

        [Key]
        public int ProductID { get; set; }
        [Required]
        public string Name { get; set; }

        public string Type { get; set; }

        [Display(Name = "Category")]
        [Required]
        public string CategoryID { get; set; }

        [Display(Name = "Sub Category")]
        public string SubCategoryId { get; set; }

        [Display(Name="UoM")]
        [Required]
        public string UOM { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
        public string ItemCategoryCode { get; set; }

        [Display(Name="Inventory Item")]
        public string ItemCode { get; set; }

        public decimal Price { get; set; }
        [NotMapped]
        public bool HasTransaction { get; set; }
    }


}
