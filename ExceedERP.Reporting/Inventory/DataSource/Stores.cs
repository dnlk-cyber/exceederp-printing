using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class Stores
    {
       ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InventoryStore> GetStores()
            {
            List<InventoryStore> stores = db.InventoryStores.ToList();
            return stores; 
            }
        
    }
}
