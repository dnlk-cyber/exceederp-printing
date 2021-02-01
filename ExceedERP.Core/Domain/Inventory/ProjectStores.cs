using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Inventory
{
   public  class ProjectStores
    {
       public int ProjectStoresId { get; set;  }
       public int ProjectId { get; set;  }
       public string StoreCode { get; set;  }
    }
}
