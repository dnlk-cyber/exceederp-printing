using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    [Table("OrganizationCategory")]
    public class OrganizationCategory : TrackUserOperation
    {
        public int OrganizationCategoryID { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Index { get; set; }

        public string Parent { get; set; }


        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }


}
