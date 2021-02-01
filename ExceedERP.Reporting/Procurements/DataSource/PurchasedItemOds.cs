using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class PurchasedItemOds : List<PurchasedItemVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PurchaseItemType()
        {
            List<string> list = new List<string>();
            list.Add("STOCK");
            list.Add("FIXED_ASSET");
            list.Add("SERVICE");
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchasedItemVm> PurchasedItem(DateTime from, DateTime to, string type)
        {
            List<PurchasedItemVm> list = new List<PurchasedItemVm> { };

            if (String.IsNullOrEmpty(type) || type == "STOCK")
            {
                #region stock
                var inventoryData = _db.GoodReceiveItems
                .Where(x => x.GoodReceive.IsOnlineApproved)
                .Where(x => x.GoodReceive.IsVoid == false)
                .Where(x => x.GoodReceive.ReceiveDate >= from)
                .Where(x => x.GoodReceive.ReceiveDate <= to).ToList();

                foreach (var receiveItem in inventoryData)
                {
                    var goodReceiveItemRecord = new PurchasedItemVm
                    {
                        Id = receiveItem.GoodReceiveId.ToString(),
                        Supplier = receiveItem.GoodReceive.SupplierName,
                        Reference = !String.IsNullOrEmpty(receiveItem.GoodReceive.ReceiveRef) ? receiveItem.GoodReceive.ReceiveRef : receiveItem.GoodReceive.Reference,
                        ReceiveDate = receiveItem.GoodReceive.ReceiveDate,
                        ItemDescription = receiveItem.ItemCode + "-" + receiveItem.InventoryItem.Name,
                        Quantity = receiveItem.Quantity,
                        UnitPrice = receiveItem.UnitPrice,
                        SubTotal = receiveItem.SubTotal,
                        GLTaxRateID = receiveItem.GLTaxRateID,
                        TaxAmount = receiveItem.TaxAmount,
                        TotalAmount = receiveItem.Total,
                        StoreName = receiveItem.GoodReceive.StoreCode
                    };

                    list.Add(goodReceiveItemRecord);
                }
                #endregion
            }

            if (String.IsNullOrEmpty(type) || type == "FIXED_ASSET")
            {
                #region fixed
                var fixedassetData = _db.FixedAssetReceiveItems
                .Where(x => x.FixedAssetReceive.IsOnlineApproved)
                .Where(x => x.FixedAssetReceive.IsVoid == false)
                .Where(x => x.FixedAssetReceive.ReceiveDate >= from)
                .Where(x => x.FixedAssetReceive.ReceiveDate <= to).ToList();

                foreach (var receiveItem in fixedassetData)
                {
                    var fixedRecord = new PurchasedItemVm
                    {
                        Id = receiveItem.FixedAssetReceiveId.ToString(),
                        Supplier = receiveItem.FixedAssetReceive.SupplierName,
                        Reference = !String.IsNullOrEmpty(receiveItem.FixedAssetReceive.ReceiveRef) ? receiveItem.FixedAssetReceive.ReceiveRef : receiveItem.FixedAssetReceive.Reference,
                        ReceiveDate = receiveItem.FixedAssetReceive.ReceiveDate,
                        ItemDescription = receiveItem.FixedAssetSubCategory.SubCategoryCode + "-" + receiveItem.FixedAssetSubCategory.SubCategoryName + "-" + receiveItem.Name,
                        Quantity = receiveItem.Quantity,
                        UnitPrice = receiveItem.UnitPrice,
                        SubTotal = receiveItem.SubTotal,
                        GLTaxRateID = receiveItem.GLTaxRateID,
                        TaxAmount = receiveItem.TaxAmount,
                        TotalAmount = receiveItem.Total,
                        StoreName = receiveItem.FixedAssetReceive.StoreCode
                    };

                    list.Add(fixedRecord);
                }
                #endregion
            }

            if (String.IsNullOrEmpty(type) || type == "SERVICE")
            {
                #region service
                var serviceData = _db.ServiceReceiveItems
                .Where(x => x.ServiceReceive.IsOnlineApproved)
                .Where(x => x.ServiceReceive.IsVoid == false)
                .Where(x => x.ServiceReceive.ReceiveDate >= from)
                .Where(x => x.ServiceReceive.ReceiveDate <= to).ToList();

                foreach (var receiveItem in serviceData)
                {
                    var fixedRecord = new PurchasedItemVm
                    {
                        Id = receiveItem.ServiceReceiveId.ToString(),
                        Supplier = receiveItem.ServiceReceive.SupplierName,
                        Reference = !String.IsNullOrEmpty(receiveItem.ServiceReceive.ReceiveRef) ? receiveItem.ServiceReceive.ReceiveRef : receiveItem.ServiceReceive.Reference,
                        ReceiveDate = receiveItem.ServiceReceive.ReceiveDate,
                        ItemDescription = receiveItem.ServiceItemCode + "-" + receiveItem.ServiceItemName ,
                        Quantity = receiveItem.Quantity,
                        UnitPrice = receiveItem.UnitPrice,
                        SubTotal = receiveItem.SubTotal,
                        GLTaxRateID = receiveItem.GLTaxRateID,
                        TaxAmount = receiveItem.TaxAmount,
                        TotalAmount = receiveItem.Total
                    };

                    list.Add(fixedRecord);
                }
                #endregion
            }
            return list;
        }
    }
}