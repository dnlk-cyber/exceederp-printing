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
    public class PurchaseReturnOds : List<PurchaseReturnVm>
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseReturnVm> GetPurchaseReturnItemByDateInterval(DateTime from, DateTime to, string store, string category)
        {
            List<PurchaseReturnVm> purchaseReturnVms = new List<PurchaseReturnVm> { };
            try
            {

                var reportData = _db.PurchaseReturnItemss.Where(x => x.PurchaseReturn.IsOnlineApproved)
                    .Where(x => x.PurchaseReturn.IsVoid == false)
                    .Where(x => x.PurchaseReturn.DateCreated >= from)
                    .Where(x => x.PurchaseReturn.DateCreated <= to).ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.PurchaseReturn.StoreCode == store).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode == category).ToList();
                }
                var uniqueCate = reportData.DistinctByField(x => x.ItemCategoryCode);
                foreach (var cat in uniqueCate)
                {
                    var purchaseReturnVm = new PurchaseReturnVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        PurchaseReturnItemList = new List<PurchaseReturnItemViewModel>()
                    };

                    foreach (var item in reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                    {
                        var purchaseReturnItemRecord = new PurchaseReturnItemViewModel
                        {
                            ReturnDate = item.PurchaseReturn.DateCreated,
                            InventoryItem = item.InventoryItem.Name,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            SubTotal = item.SubTotal,
                            GLTaxRateID = item.GLTaxRateID,
                            TaxAmount = item.TaxAmount,
                            TotalAmount = item.TotalAmount,
                            StoreName = item.PurchaseReturn.StoreCode
                        };

                        purchaseReturnVm.PurchaseReturnItemList.Add(purchaseReturnItemRecord);
                    }
                    purchaseReturnVm.CategoryTotal = purchaseReturnVm.PurchaseReturnItemList.Sum(x => x.TotalAmount);
                    purchaseReturnVms.Add(purchaseReturnVm);
                }

                return purchaseReturnVms;
            }
            catch (Exception)
            {
                return purchaseReturnVms;
            }
            
        }
    }
}