using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.ApplicationServices.Inventory
{
   public  class InventoryItemService
    {
        private  ExceedDbContext db = new ExceedDbContext();

        public  int GetStoreBranch(string storecode)
        {
            int branchid = 0;
            var record = db.InventoryStores.FirstOrDefault(x => x.StoreCode == storecode);
            if (record != null) branchid = record.BranchId;
            return branchid;
        }

        public  decimal GetStoreItemBalance(string itemcode, string storecode)
        {
            decimal balance = 0;
            var record = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).AsNoTracking().FirstOrDefault(x => x.ItemCode == itemcode);
            if (record != null) balance = record.Balance;
            return balance;
        }
        public  decimal GetBranchItemAveragePrice(int branchid, string itemcode)
        {
            decimal averagePrice = 0;
            var record = db.BranchItemAssignments.Where(x => x.BranchId == branchid).FirstOrDefault(x => x.ItemCode == itemcode);
            if (record != null) averagePrice = record.AvgPrice;
            return averagePrice;
        }
        public  decimal GetItemAveragePrice(string itemcode, string storecode)
        {
            decimal avgPrice = 0;


            #region get average price
            var inventoryItem = db.InventoryItems.Find(itemcode);

            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
            {
                if (inventoryItem != null) avgPrice = inventoryItem.AvgPrice;
            }
            else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
            {
                int storeBranch = GetStoreBranch(storecode);
                if (inventoryItem != null)
                {
                    var branchPrice = GetBranchItemAveragePrice(storeBranch, inventoryItem.ItemCode);

                    avgPrice = branchPrice;
                }
            }
            else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
            {
                var storeItemAssignment = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).FirstOrDefault(x => x.ItemCode == inventoryItem.ItemCode);
                if (storeItemAssignment != null)
                {
                    var storePrice = storeItemAssignment.AvgPrice;
                    avgPrice = storePrice;
                }
            }

            #endregion


            return avgPrice;
        }

        public  void DefineBranchItems(int id)
        {
            var branchs = db.Branch.Where(x => x.BranchId == id).ToList();
            var stores = db.InventoryStores.ToList();
            foreach (var branch in branchs)
            {
                var branchStores = stores.Where(x => x.BranchId == branch.BranchId).Select(x => x.StoreCode).ToList();
                var branchStoreItems = db.StoreItemAssignments.Where(x => branchStores.Contains(x.StoreCode)).ToList();
                foreach (var item in branchStoreItems)
                {
                    var item1 = item;
                    var exist = db.BranchItemAssignments.Where(x => x.BranchId == id).Where(x => x.ItemCode == item1.ItemCode).ToList();
                    if (!exist.Any())
                    {
                        #region define branch item

                        var branchItem = new BranchItemAssignment
                        {
                            BranchId = branch.BranchId,
                            ItemCategoryCode = item.ItemCategoryCode,
                            ItemCode = item.ItemCode,
                            Balance = 0,
                            AvgPrice = 0
                        };
                        db.BranchItemAssignments.Add(branchItem);
                        db.SaveChanges();
                        #endregion
                    }
                }
            }

        }
        public void UpdateAllBranchItems(int id)
        {
            DefineBranchItems(id);
            var branch = db.Branch.Find(id);
            var branchItems = db.BranchItemAssignments.ToList();
            var storeitems = db.StoreItemAssignments.ToList();
            if (branch != null)
            {
                var brachStores = db.InventoryStores.Where(x => x.BranchId == branch.BranchId).Select(x => x.StoreCode).ToList();
                var branchStoreItems = storeitems.Where(x => brachStores.Contains(x.StoreCode)).ToList();
                var selectedbranchItems = branchItems.Where(x => x.BranchId == branch.BranchId).ToList();
                foreach (var branchItem in selectedbranchItems)
                {
                    decimal avg = 0;
                    decimal price = 0;
                    decimal qty = 0;

                    var bitem = branchItem;
                    var itemInstances = branchStoreItems.Where(x => x.ItemCode == bitem.ItemCode).ToList();

                    qty = itemInstances.Sum(x => x.Balance);

                    foreach (var item in itemInstances)
                    {
                        price = price + item.Balance * item.AvgPrice;
                    }
                    if (qty != 0)
                    {
                        avg = price / qty;
                    }

                    branchItem.Balance = qty;
                    branchItem.AvgPrice = avg;
                    db.Entry(branchItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

        }
        public void UpdateAllInventoryItems()
        {
            var inventoryItems = db.InventoryItems.ToList();
            var storeitems = db.StoreItemAssignments.ToList();
            foreach (var item in inventoryItems)
            {
                item.Balance = storeitems.Where(x => x.ItemCode == item.ItemCode).Sum(x => x.Balance);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public  void CreateBeginning(string storecode)
        {
            var storeitems = db.StoreItemAssignments.Where(x => x.StoreCode == storecode).ToList();
            foreach (var item in storeitems)
            {
                var exist = db.BinCardLines.Where(x => x.StoreItemAssignmentID == item.StoreItemAssignmentID).ToList();
                if (!exist.Any())
                {
                    #region bin card line

                    BinCardLine binCardLine = new BinCardLine
                    {
                        StoreItemAssignment = item,
                        ReferenceDoc = "Beginning",
                        Date = new DateTime(2017, 4, 30),
                        BalanceQTY = item.Balance,
                        BalanceUPrice = item.AvgPrice
                    };
                    binCardLine.BalanceTPrice = item.AvgPrice * binCardLine.BalanceQTY;
                    db.BinCardLines.Add(binCardLine);
                    db.SaveChanges();

                    #endregion
                }
            }
        }
    }
}
