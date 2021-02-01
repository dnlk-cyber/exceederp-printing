using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class DeadStockOds : List<DeadStockVm>
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DeadStockVm> GetDeadStocks(string store,string category)
        {
            List<DeadStockVm> list = new List<DeadStockVm> { };
            DateTime today = DateTime.Today;
            DateTime beforeYear = today.AddYears(-1);

            #region items in store but no movement
            var deadStoreItems = _db.StoreItemAssignments
                .Where(x => x.BinCardLines.Where(b => b.Date >= beforeYear).Where(b=>b.ReferenceDoc.StartsWith("Count") ==false).Any() == false)
                .ToList();

            if (!string.IsNullOrEmpty(store))
                deadStoreItems = deadStoreItems.Where(x=>x.StoreCode==store).ToList();
            if (!string.IsNullOrEmpty(category))
                deadStoreItems = deadStoreItems.Where(x => x.ItemCategoryCode == category).ToList();

            var uniqueDeads = deadStoreItems.DistinctByField(x => x.ItemCode);

            foreach (var dead in uniqueDeads)
            {
                var record = new DeadStockVm();
                record.ItemCode = dead.ItemCode;
                record.Category = dead.ItemCategory.Name;
                record.Description = dead.InventoryItem.Name;
                record.Unit = dead.InventoryItem.UoMName;
                record.PartNo = dead.InventoryItem.PartNo;
                record.Balance = deadStoreItems.Where(x=>x.ItemCode==dead.ItemCode).Sum(x=>x.Balance);

                list.Add(record);
            }
            #endregion
            #region items not assigned to any store
            var itemsNotInStore = _db.InventoryItems.Where(x => x.StoreItemAssignments.Any() == false).ToList();
            foreach (var dead in itemsNotInStore)
            {
                var record = new DeadStockVm();
                record.ItemCode = dead.ItemCode;
                record.Category = dead.ItemCategory.Name;
                record.Description = dead.Name;
                record.Unit = dead.UoMName;
                record.PartNo = dead.PartNo;
                record.Balance = 0;

                list.Add(record);
            }
            #endregion
            return list;
        }
    }
}