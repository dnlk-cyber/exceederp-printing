using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class Store : TrackUserOperation
    {
        public int StoreID { get; set; }
        [Required]
        public string Description { get; set; }


        [DisplayName("Parent")]
        public string ParentId { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Abbrivation { get; set; }

        public string Remark { get; set; }
    }

}
