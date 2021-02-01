using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    [Table("PersonCategory")]
    public class PersonCategory : TrackUserOperation
    {
        public int PersonCategoryID { get; set; }

        public string Description { get; set; }

        public int Index { get; set; }

        public int? Parent { get; set; }


        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }

   
}
