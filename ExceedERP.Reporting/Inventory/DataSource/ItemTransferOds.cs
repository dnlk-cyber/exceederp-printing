using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class ItemTransferOds : List<ItemTransferVm>
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ItemTransferVm> GetTransferItemByDateInterval(DateTime from, DateTime to, string store, string tostore, string category,string item)
        {
            List<ItemTransferVm> itemTransferList = new List<ItemTransferVm> { };
            try
            {

                var reportData = _db.ItemTransferItems
                    .Where(x => x.ItemTransfer.IsOnlineApproved)
                    .Where(x => x.ItemTransfer.IsVoid == false)
                    .Where(x => x.ItemTransfer.TransferDate >= from)
                    .Where(x => x.ItemTransfer.TransferDate <= to).ToList();
                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData.Where(x => x.ItemTransfer.FromStore == store).ToList();
                }
                if (!String.IsNullOrEmpty(tostore))
                {
                    reportData = reportData.Where(x => x.ItemTransfer.ToStore == tostore).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode == category).ToList();
                }

                if (!String.IsNullOrEmpty(item))
                {
                    reportData = reportData.Where(x => x.ItemCode == item).ToList();
                }
                var uniqueCate = reportData.DistinctByField(x => x.ItemCategoryCode);
                foreach (var cat in uniqueCate)
                {

                    var transferVm = new ItemTransferVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotalSent = 0,
                        CategoryTotalReceived = 0,
                        ItemTransferList = new List<ItemTransferViewModel>()
                    };
                     
                    foreach (var transferItem in reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                    {
                        var fromStore = _db.InventoryStores.Find(transferItem.ItemTransfer.FromStore);
                        var toStore = _db.InventoryStores.Find(transferItem.ItemTransfer.ToStore);

                        
                        var receivedList = _db.ItemTransferItemReceive
                            .Where(x => x.ItemTransferReceive.ItemTransferID == transferItem.ItemTransferID)
                            .Where(x => x.ItemTransferReceive.IsOnlineApproved)
                            .Where(x => x.ItemTransferReceive.IsVoid == false)
                            .Where(x=>x.ItemCode==transferItem.ItemCode)
                            .ToList();

                        var itemTransferRecord = new ItemTransferViewModel
                        {
                            Id = transferItem.ItemTransferID.ToString(),
                            Reference = transferItem.ItemTransfer.DocumentRef,
                            TransferDate = transferItem.ItemTransfer.TransferDate,
                            InventoryItem = transferItem.ItemCode + "-" + transferItem.InventoryItem.Name,
                            ShippedQuantity = transferItem.ShippedQuantity,
                            UPrice= transferItem.UPrice,
                            TPrice = transferItem.TPrice,
                            ReceivedQuantity = receivedList.Sum(x=>x.ReceivedQuantity),

                        };
                        if (fromStore != null && toStore != null)
                        {
                            itemTransferRecord.FromStore = transferItem.ItemTransfer.FromStore + "-" + fromStore.Name;
                            itemTransferRecord.ToStore = transferItem.ItemTransfer.ToStore + "-" + toStore.Name;
                        }
                        transferVm.ItemTransferList.Add(itemTransferRecord);
                    }
                    transferVm.CategoryTotalSent = transferVm.ItemTransferList.Sum(x => x.ShippedQuantity);
                    transferVm.CategoryTotalReceived = transferVm.ItemTransferList.Sum(x => x.ReceivedQuantity);
                    itemTransferList.Add(transferVm);
                }
                return itemTransferList;
            }
            catch (Exception)
            {
                return itemTransferList;
            }
           
        }
    }
}