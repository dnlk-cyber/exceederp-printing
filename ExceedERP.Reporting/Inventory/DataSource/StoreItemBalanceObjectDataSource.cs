using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class StoreItemBalanceObjectDataSource : List<StockBalanceViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<StockBalanceViewModel> GetStockBalanceByCategory(string category, string store)
        {
            List<StockBalanceViewModel> stockBalanceList = new List<StockBalanceViewModel> { };
            
            if (!string.IsNullOrEmpty(store))
            {
                var storeItemAssignments = _db.StoreItemAssignments.Where(x => x.StoreCode == store).AsQueryable();

                if (!String.IsNullOrEmpty(category))
                {
                    storeItemAssignments =
                        storeItemAssignments.Where(x => x.ItemCategoryCode == category).AsQueryable();
                }
               
                foreach (var item in storeItemAssignments)
                {
                    var stockBalanceRecord = new StockBalanceViewModel
                    {
                        ItemCategory = item.ItemCategory.Name,
                        ItemCode = item.InventoryItem.ItemCode,
                        InventoryItem =  item.InventoryItem.Name,
                        ItemLocation = item.ItemLocation,
                        Balance = item.Balance,
                        AvgPrice = item.AvgPrice,
                        TotalAmount = item.Balance * item.AvgPrice
                    };
                    stockBalanceList.Add(stockBalanceRecord);
                }
            }
           

            return stockBalanceList.OrderBy(x=>x.ItemCode);
            
        }
    }
}
