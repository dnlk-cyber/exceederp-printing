using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
   
    public class GoodSellingAndLendingItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

          [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult GoodSellingAndLendingItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<GoodSellingAndLendingItem> goodsellingandlendingItems = _db.GoodSellingAndLendingItems.Where(gsl => gsl.GoodSellingAndLendingID == id);
            DataSourceResult result = goodsellingandlendingItems.ToDataSourceResult(request, goodSellingAndLendingItem => new {
                GoodSellingAndLendingItemID = goodSellingAndLendingItem.GoodSellingAndLendingItemID,
                GoodSellingAndLendingID = goodSellingAndLendingItem.GoodSellingAndLendingID,
                ItemCategoryCode = goodSellingAndLendingItem.ItemCategoryCode,
                ItemCode = goodSellingAndLendingItem.ItemCode,
                QuantitySold = goodSellingAndLendingItem.QuantitySold,
                UnitPurchasingPrice = goodSellingAndLendingItem.UnitPurchasingPrice,
                UnitSellingPrice = goodSellingAndLendingItem.UnitSellingPrice,
                TotalSellingPrice = goodSellingAndLendingItem.TotalSellingPrice,
                GLAccountCode = goodSellingAndLendingItem.GLAccountCode,
                OrgBranchName = goodSellingAndLendingItem.OrgBranchName,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingItems_Create([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingItem goodSellingAndLendingItem , int id )
        {
            var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == goodSellingAndLendingItem.ItemCode);
            var goodSellingAndLending = _db.GoodSellingAndLendings.Find(id);
            decimal avgPrice = 0;
            if (goodSellingAndLending != null)
                avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(goodSellingAndLendingItem.ItemCode, goodSellingAndLending.StoreCode);
            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new GoodSellingAndLendingItem
                    {
                    
                        GoodSellingAndLendingID = id,
                        ItemCategoryCode = goodSellingAndLendingItem.ItemCategoryCode,
                        ItemCode = goodSellingAndLendingItem.ItemCode,
                        QuantitySold = goodSellingAndLendingItem.QuantitySold,
                        UnitPurchasingPrice = avgPrice,
                        UnitSellingPrice = goodSellingAndLendingItem.UnitSellingPrice,
                        TotalSellingPrice = goodSellingAndLendingItem.QuantitySold * goodSellingAndLendingItem.UnitSellingPrice,
                        GLAccountCode = item.ItemCategory.AccountCode,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                   
                    };

                    _db.GoodSellingAndLendingItems.Add(entity);
                    _db.SaveChanges();
                    goodSellingAndLendingItem.GoodSellingAndLendingItemID = entity.GoodSellingAndLendingItemID;
                    GoodSalesDistribution(_db.GoodSellingAndLendings.Find(entity.GoodSellingAndLendingID));

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good selling item", "Success", User.Identity.Name, $"created good selling item {entity.ItemCode} under record {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { goodSellingAndLendingItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingItems_Update([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingItem goodSellingAndLendingItem , int id)
        {
            var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == goodSellingAndLendingItem.ItemCode);
            var goodSellingAndLending = _db.GoodSellingAndLendings.Find(id);
            decimal avgPrice = 0;
            if (goodSellingAndLending != null)
                avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(goodSellingAndLendingItem.ItemCode, goodSellingAndLending.StoreCode);
            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new GoodSellingAndLendingItem
                    {
                        GoodSellingAndLendingItemID = goodSellingAndLendingItem.GoodSellingAndLendingItemID,
                        GoodSellingAndLendingID =  id,
                        ItemCategoryCode = goodSellingAndLendingItem.ItemCategoryCode,
                        ItemCode = goodSellingAndLendingItem.ItemCode,
                        QuantitySold = goodSellingAndLendingItem.QuantitySold,
                        UnitPurchasingPrice = avgPrice,
                        UnitSellingPrice = goodSellingAndLendingItem.UnitSellingPrice,
                        TotalSellingPrice = goodSellingAndLendingItem.QuantitySold * goodSellingAndLendingItem.UnitSellingPrice,
                        GLAccountCode = item.ItemCategory.AccountCode,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
      
                    };

                    _db.GoodSellingAndLendingItems.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good selling item", "Success", User.Identity.Name, $"updated good selling item {entity.ItemCode} under record {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                    #endregion
                }

                _db.SaveChanges();
                GoodSalesDistribution(_db.GoodSellingAndLendings.Find(id));

            }

            return Json(new[] { goodSellingAndLendingItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingItems_Destroy([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingItem goodSellingAndLendingItem , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLendingItem
                {
                    GoodSellingAndLendingItemID = goodSellingAndLendingItem.GoodSellingAndLendingItemID,
                    ItemCode = goodSellingAndLendingItem.ItemCode,
                    QuantitySold = goodSellingAndLendingItem.QuantitySold,
                    UnitPurchasingPrice = goodSellingAndLendingItem.UnitPurchasingPrice,
                    UnitSellingPrice = goodSellingAndLendingItem.UnitSellingPrice,
                    TotalSellingPrice = goodSellingAndLendingItem.TotalSellingPrice,
                    GLAccountCode = goodSellingAndLendingItem.GLAccountCode,
                    OrgBranchName = goodSellingAndLendingItem.OrgBranchName,
                };

                _db.GoodSellingAndLendingItems.Attach(entity);
                _db.GoodSellingAndLendingItems.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted inventory sales item " + goodSellingAndLendingItem.GoodSellingAndLendingID + " " + goodSellingAndLendingItem.ItemCode);
                #endregion
                GoodSalesDistribution(_db.GoodSellingAndLendings.Find(id));

            }

            return Json(new[] { goodSellingAndLendingItem }.ToDataSourceResult(request, ModelState));
        }

     
        private void GoodSalesDistribution(GoodSellingAndLending record)
        {
            GoodSellingAndLendingDistribution distribution;
            if (record.GoodSellingAndLendingDistribution != null)
            {

                //  db.GoodReceiveDistributions.Attach(grDistribution);
                _db.GoodSellingAndLendingDistributions.RemoveRange(record.GoodSellingAndLendingDistribution);
                _db.SaveChanges();
            }
            #region Debit
            foreach (var grSlItem in record.GoodSellingAndLendingItems)
            {
                InventoryItem item = _db.InventoryItems.Find(grSlItem.ItemCode);
                ItemCategory category = _db.ItemCategorys.Find(item.ItemCategoryCode);
                string accountCode = category.AccountCode;
                string[] accSplit = accountCode.Split('-');
                string costCenter = "00000";
                string newAccountCode = accSplit[0] + "-" + accSplit[1] + "-" + costCenter + "-" + accSplit[3];
                distribution = new GoodSellingAndLendingDistribution();
                distribution.AccountCode = newAccountCode;
                distribution.Credit = 0;
                distribution.Debit = grSlItem.TotalSellingPrice;
                distribution.GoodSellingAndLendingID = record.GoodSellingAndLendingID;
                distribution.AdditionalDescription = "";
                _db.GoodSellingAndLendingDistributions.Add(distribution);
                _db.SaveChanges();

            }

            #endregion

            #region Credit
            foreach (var grSlItem in record.GoodSellingAndLendingItems)
            {
                InventoryItem item = _db.InventoryItems.Find(grSlItem.ItemCode);
                ItemCategory category = _db.ItemCategorys.Find(item.ItemCategoryCode);
                string accountCode = category.IssueAccountCode;
                string[] accSplit = accountCode.Split('-');
                string costCenter = "00000";
                string newAccountCode = accSplit[0] + "-" + accSplit[1] + "-" + costCenter + "-" + accSplit[3];
                distribution = new GoodSellingAndLendingDistribution();
                distribution.AccountCode = newAccountCode;
                distribution.Credit = grSlItem.TotalSellingPrice;
                distribution.Debit = 0;
                distribution.GoodSellingAndLendingID = record.GoodSellingAndLendingID;
                distribution.AdditionalDescription = "";
                _db.GoodSellingAndLendingDistributions.Add(distribution);
                _db.SaveChanges();
            #endregion
            }




        }
         

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
