using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesStore : TrackUserOperation
    {
        [DisplayName("Id")]
        public int SalesStoreId { get; set; }

        public string StoreCode { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

       
        public virtual InventoryStore InventoryStore { get; set; }
    }
}
