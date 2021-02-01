using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class ItemPerStoreOds : List<ItemPerStore>
        {
             ExceedDbContext _db = new ExceedDbContext();
             List<ItemPerStore> _itemInStores = new List<ItemPerStore>();
        
            [DataObjectMethod(DataObjectMethodType.Select)]
            public List<ItemPerStore> ItemPerStores(string item)
                {
                    var storeItemAssignment = _db.StoreItemAssignments.Where(x=>x.ItemCode==item).ToList();
                    foreach (var items in storeItemAssignment)
                    {
                        var perStore = new ItemPerStore
                        {
                            ItemName = items.InventoryItem.Name,
                            Store = items.InventoryStore == null ? items.StoreCode : items.InventoryStore.Name + "-" + items.StoreCode,
                            Count =items.Balance
                               
                        };
                        _itemInStores.Add(perStore);
                    }
            
                    return _itemInStores; 

                 }
       }
}
