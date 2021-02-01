using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventory.DataSource
{

    [DataObject]
   public class CategoryItemCustomOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ItemCategory> GetItemCategories()
            {
            return _db.ItemCategorys.ToList(); 
            }

       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InventoryItem> GetInventoryItems()
            {
            return _db.InventoryItems.ToList(); 
            }
    }
}
