using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
    [DataObject]
    public class PurchaseDetailListOds : List<FixedAssetPurchaseItemVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetPurchaseItemVm> GoodsReceiveByDateInterval(DateTime from, DateTime to, string store, string category, string businessUnitId)
        {
            List<FixedAssetPurchaseItemVm> goodReceiveItemList = new List<FixedAssetPurchaseItemVm> { };
            try
            {

                var reportData = _db.FixedAssetReceiveItems
                     .Where(x => x.FixedAssetReceive.IsOnlineApproved)
                     .Where(x => x.FixedAssetReceive.IsVoid == false)
                     .Where(x => x.FixedAssetReceive.ReceiveDate >= from.Date)
                     .Where(x => x.FixedAssetReceive.ReceiveDate <= to.Date).AsQueryable();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.FixedAssetReceive.StoreCode == store).AsQueryable();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.FixedAssetCategory.CategoryCode == category).AsQueryable();
                }

				if (!String.IsNullOrEmpty(businessUnitId))
				{
					int did = int.Parse(businessUnitId);
					List<string> stores=InventoryHelper.GetStoreByBusinessUnit(did);
                        
					reportData = reportData.Where(x => stores.Contains(x.FixedAssetReceive.StoreCode));
				}


				foreach (var item in reportData)
                {
                    var goodReceiveItemRecord = new FixedAssetPurchaseItemVm
                    {
                        Id = item.FixedAssetReceiveId.ToString(),
                        Reference= item.FixedAssetReceiveId.ToString(),
                        GRNNo = !string.IsNullOrEmpty(item.FixedAssetReceive.ReceiveRef) ? item.FixedAssetReceive.ReceiveRef : item.FixedAssetReceive.Reference,
                        ReceiveDate = item.FixedAssetReceive.ReceiveDate,
                        Category=item.FixedAssetCategory.CategoryName,
                        SubCategory=item.FixedAssetSubCategory.SubCategoryName,
                        Name=item.Name,
                        TagNo=item.TagNo,
                        UoM=item.Unit,
                        Supplier=item.FixedAssetReceive.SupplierName,
                        SerialNo=item.SerialNo,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.Quantity* item.UnitPrice,
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,
                        TotalAmount = item.Total,
                        StoreName = item.FixedAssetReceive.StoreCode
                    };

                    goodReceiveItemList.Add(goodReceiveItemRecord);
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