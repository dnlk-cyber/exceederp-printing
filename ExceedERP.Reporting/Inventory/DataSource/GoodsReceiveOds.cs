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
    public class GoodsReceiveOds : List<GoodReceiveVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GoodReceiveVm> GoodsReceiveByDateInterval(DateTime from, DateTime to, string store, string category,string item)
        {
            List<GoodReceiveVm> goodReceiveItemList = new List<GoodReceiveVm> { };
            try
            {

               var reportData = _db.GoodReceiveItems
                    .Where(x => x.GoodReceive.IsOnlineApproved)
                    .Where(x => x.GoodReceive.IsVoid == false)
                    .Where(x => x.GoodReceive.ReceiveDate >= from)
                    .Where(x => x.GoodReceive.ReceiveDate <= to).ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.GoodReceive.StoreCode == store).ToList();
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
                    var goodVm = new GoodReceiveVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        GoodReceiveItemList = new List<GoodReceiveItemViewModel>()
                    };

                    foreach (var receiveItem in reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                    {
                        var goodReceiveItemRecord = new GoodReceiveItemViewModel
                        {
                            Id = receiveItem.GoodReceiveId.ToString(),
                            Supplier = receiveItem.GoodReceive.SupplierName,
                            Reference = !String.IsNullOrEmpty(receiveItem.GoodReceive.ReceiveRef) ? receiveItem.GoodReceive.ReceiveRef : receiveItem.GoodReceive.Reference,
                            ReceiveDate = receiveItem.GoodReceive.ReceiveDate,
                            InventoryItem = receiveItem.ItemCode + "-" + receiveItem.InventoryItem.Name,
                            Quantity = receiveItem.Quantity,
                            UnitPrice = receiveItem.UnitPrice,
                            SubTotal = receiveItem.SubTotal,
                            GLTaxRateID = receiveItem.GLTaxRateID,
                            TaxAmount = receiveItem.TaxAmount,
                            TotalAmount = receiveItem.Total,
                            StoreName = receiveItem.GoodReceive.StoreCode
                        };

                        goodVm.GoodReceiveItemList.Add(goodReceiveItemRecord);
                    }
                    goodVm.CategoryTotal = goodVm.GoodReceiveItemList.Sum(x => x.TotalAmount);
                    goodReceiveItemList.Add(goodVm);
                }

                return goodReceiveItemList;
            }
            catch (Exception)
            {
                return goodReceiveItemList;
            }
        }
    }
}