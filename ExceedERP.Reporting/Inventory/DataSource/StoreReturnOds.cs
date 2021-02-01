using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class StoreReturnOds : List<StoreReturnItemViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StoreReturnVm> GetStoreReturnItemByDateInterval(DateTime from, DateTime to, string store, string category,string item)
        {
            List<StoreReturnVm> storeReturnItemList = new List<StoreReturnVm> { };

            try
            {
                var reportData = _db.StoreReturnItemss.Where(x => x.StoreReturn.IsOnlineApproved)
                    .Where(x => x.StoreReturn.ReturnDate >= from)
                    .Where(x => x.StoreReturn.ReturnDate <= to)
                    .Where(x => x.StoreReturn.IsVoid == false).ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.StoreReturn.StoreCode == store).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode == category).ToList();
                }

                if (!String.IsNullOrEmpty(item))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode == item).ToList();
                }

                var uniqueCate = reportData.DistinctByField(x => x.ItemCategoryCode);
                foreach (var cat in uniqueCate)
                {
                    var goodVm = new StoreReturnVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        StoreReturnItemList = new List<StoreReturnItemViewModel>()
                    };
                    var returnItems = reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode);
                    foreach (var returnItem in returnItems)
                    {
                        int branchid = returnItem.StoreReturn.IsForOther
                            ? (int) returnItem.StoreReturn.ForBranchId
                            : InventoryHelper.GetStoreBranch(returnItem.StoreReturn.StoreCode);
                        var storeReturnItemRecord = new StoreReturnItemViewModel
                        {
                            Id = returnItem.StoreReturnID.ToString(),
                            Reference = returnItem.StoreReturn.Remark,
                            BranchOrProject=InventoryPrintHelper.BranchName(branchid),
                            WorkUnit = InventoryPrintHelper.WorkUnitName(returnItem.StoreReturn.OrgStructureId),
                            ReturnDate = returnItem.StoreReturn.ReturnDate,
                            InventoryItem = returnItem.InventoryItem.Name,
                            Quantity = returnItem.Quantity,
                            Price=returnItem.UnitPrice,
                            TotalPrice=returnItem.UnitPrice * returnItem.Quantity,
                            StoreName = returnItem.StoreReturn.StoreCode
                        };
                        goodVm.StoreReturnItemList.Add(storeReturnItemRecord);
                    }
                    goodVm.CategoryTotal = goodVm.StoreReturnItemList.Sum(x => x.Quantity);
                    storeReturnItemList.Add(goodVm);
                }

                return storeReturnItemList;
            }
            catch (Exception)
            {
                return storeReturnItemList;
            }
        }
    }
}