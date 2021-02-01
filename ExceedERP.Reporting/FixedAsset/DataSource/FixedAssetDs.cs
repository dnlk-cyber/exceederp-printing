using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
   public  class FixedAssetDs
    {
    }

  
    [DataObject]
    public class DepreciationDs
        {
        ExceedDbContext _db = new ExceedDbContext();
        public List<Depreciation> Depreciations = new List<Depreciation>();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Depreciation> GetDepreciations(DateTime to , string store)
            {
                foreach (var fixedAsset in _db.AssetRegistrations.ToList())
                    {
                        int goodReceiveid = int.Parse(fixedAsset.GRN);
                        GoodReceiveItem grGoodReceiveItem = _db.GoodReceiveItems.Find(goodReceiveid);
                        if (grGoodReceiveItem.GoodReceive.StoreCode == store)
                            {
                                int dateDiff = 0;
                                if (fixedAsset.GRNDate != null)
                                {
                                    DateTime grnd = (DateTime)fixedAsset.GRNDate;
                                    TimeSpan ts = to - grnd;
                                    dateDiff = ts.Days;
                                }
                                InventoryItem iventoryItem = _db.InventoryItems.Find(fixedAsset.Name);
                                ItemCategory itemCategory = _db.ItemCategorys.Find(iventoryItem.ItemCategoryCode);
                                ItemCategory parentCategory = null;
                                DepreciationRule record;
                                Depreciation depret = new Depreciation();



                                if (itemCategory.HaveParent)
                                {

                                    parentCategory = _db.ItemCategorys.Find(itemCategory.ParentName);
                                }

                                #region Check For Operation

                                //if (parentCategory != null)
                                //{
                                //    record = _db.DepreciationRules.FirstOrDefault(x => x.SubCategoryCode == itemCategory.ItemCategoryCode && x.ItemCategoryCode == parentCategory.ItemCategoryCode);
                                //    depret.CategoryName = parentCategory.Name;
                                //    depret.SubCategoryName = itemCategory.Name;

                                //}
                                //else
                                //{
                                //    record =
                                //        _db.DepreciationRules.FirstOrDefault(
                                //            x => x.ItemCategoryCode == itemCategory.ItemCategoryCode && x.FixedAssetGroup.DepreciationMethod == DepreciationMethod.Pooling);
                                //    depret.CategoryName = itemCategory.Name;

                                //}
                                #endregion

                                record =
                                    _db.DepretiationRules.FirstOrDefault(x => x.CategoryCode == fixedAsset.FixedAssetCategory.CategoryCode);

                                if (record != null)
                                {
                                    decimal accomodatedValue = (decimal)(record .FixedAssetGroup.PoolingPercentage* (float)fixedAsset.BookValue) * dateDiff / 36500;
                                    depret.ItemName = iventoryItem.Name;
                                    depret.InitialPrice = fixedAsset.PurchasePrice; 
                                    depret.BookValue =Math.Round((fixedAsset.PurchasePrice - accomodatedValue),2);
                                    depret.Accomodate = Math.Round(accomodatedValue , 2);
                                }


                                Depreciations.Add(depret); 
                            }

                }
                        
            return Depreciations; 
            }
        }
public class Depreciation
    {
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public string SubCategoryName { get; set; }
    public decimal InitialPrice { get; set;  }
    public decimal BookValue { get; set;  }
    public decimal Accomodate { get; set;  }
    }
}
