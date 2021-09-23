using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    [Table("ThingCategory")]

    public class ThingCategory : TrackUserOperation
    {
        public int ThingCategoryID { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Index { get; set; }

        public string Parent { get; set; }

        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }

}
