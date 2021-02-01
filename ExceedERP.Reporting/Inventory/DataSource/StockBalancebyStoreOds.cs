using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Inventory.DataSource
{

    [DataObject]
    public class StockBalancebyStoreOds : List<StockBalalnceByStoreVm>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StockBalalnceByStoreVm> GetStoreBalance(string store,string catcode,string type)
        {
            var list = new List<StockBalalnceByStoreVm>();

            var branches = db.Branch.AsQueryable();
            var catagories = db.ItemCategorys.OrderBy(x => x.Name).AsQueryable();
            var items = db.InventoryItems.OrderBy(x => x.Name).AsQueryable();

            if (!String.IsNullOrEmpty(catcode))
                catagories = catagories.Where(x => x.ItemCategoryCode == catcode).AsQueryable();           

            //if (!String.IsNullOrEmpty(type))
            //    catagories = catagories.Where(x => x.StockType.ToString() == type).AsQueryable();

            foreach (var catagory in catagories)
                {
                    var catItems = items.Where(x => x.ItemCategoryCode == catagory.ItemCategoryCode).AsQueryable();

                    foreach (var item in catItems)
                    {

                        foreach(var branch in branches)
                        {
                        var stores = db.InventoryStores.Where(x => x.BranchId == branch.BranchId).AsQueryable();

                        if (!String.IsNullOrEmpty(store))
                            stores = stores.Where(x => x.StoreCode == store).AsQueryable();

                        List<string> branchStores = stores.Select(x => x.StoreCode).ToList();

                        var storeItems = db.StoreItemAssignments
                            .Where(x => x.ItemCode == item.ItemCode)
                            .Where(x => x.Balance > 0)
                            .Where(x=>branchStores.Contains(x.StoreCode))
                            .AsQueryable();

                            foreach (var storeItem in storeItems)
                            {
                                var balance = new StockBalalnceByStoreVm
                                {
                                    ItemCategory = catagory.Name,
                                    ItemName = item.Name,
                                    UOM = item.UoM?.UoMName,
                                    Branch = branch.Name,
                                    Quantity = storeItem.Balance,
                                    AveragePrice = item.AvgPrice
                                };

                                list.Add(balance);
                            }

                    }                                        
                   
                    }

                }
            
            return list;
        }
    }

}