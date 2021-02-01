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
    public class StockAdjustmentOds : List<StockAdjustmenVm>
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StockAdjustmenVm> GetStockAdjustmentItemByDateInterval(DateTime from, DateTime to, string store, string category,string item)
        {
            List<StockAdjustmenVm> stockAdjustmentItemList = new List<StockAdjustmenVm> { };
            try
            {

                var reportData = _db.StockAdjustmentItemss.Where(x => x.StockAdjustment.IsOnlineApproved)
                    .Where(x => x.StockAdjustment.IsVoid == false)
                    .Where(x => x.StockAdjustment.AdjustmentDate >= from)
                    .Where(x => x.StockAdjustment.AdjustmentDate <= to)
                    .ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.StockAdjustment.StoreCode == store).ToList();
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
                    var adjustmenVm = new StockAdjustmenVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        StockAdjustmenItemList = new List<StockAdjustmentItemViewModel>()
                    };

                    foreach (var adjustmentItems in reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                    {
                        var stockAdjustmentItemRecord = new StockAdjustmentItemViewModel
                        {
                            AdjustmentDate = adjustmentItems.StockAdjustment.AdjustmentDate,
                            InventoryItem = adjustmentItems.InventoryItem.Name,
                            BinCardBalance = adjustmentItems.BinCardBalance,
                            AdjustedPrice = adjustmentItems.AdjustedPrice,
                            StockAdjustmentType = adjustmentItems.StockAdjustmentType,
                            PriceAdjustmentType = adjustmentItems.PriceAdjustmentType,
                            AdjustedQuantity = adjustmentItems.AdjustedQuantity
                        };

                        adjustmenVm.StockAdjustmenItemList.Add(stockAdjustmentItemRecord);
                    }
                    adjustmenVm.CategoryTotal = adjustmenVm.StockAdjustmenItemList.Sum(x => x.BinCardBalance);
                    stockAdjustmentItemList.Add(adjustmenVm);
                }
               
                return stockAdjustmentItemList;
            }
            catch (Exception)
            {
                return stockAdjustmentItemList;
            }
        }
    }
}