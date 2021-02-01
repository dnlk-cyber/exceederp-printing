 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Required]
        public string UOM { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }


    }


}
