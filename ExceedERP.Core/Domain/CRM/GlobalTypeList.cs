using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{

    public class GlobalTypeList
    {
        public int GlobalTypeListID { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }

        public bool IsActive { get; set; }

        public string Remark { get; set; }

    }
}
