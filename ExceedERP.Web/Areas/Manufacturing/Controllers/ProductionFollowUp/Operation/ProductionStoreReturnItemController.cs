using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreReturnItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // private readonly DistributionHelper _distributionHelper = new DistributionHelper();

      //  [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult StoreReturnItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreReturnItems> storereturnItemss = db.StoreReturnItemss.Where(sr => sr.StoreReturnID == id);
            DataSourceResult result = storereturnItemss.ToDataSourceResult(request, storeReturnItems => new
            {
                StoreReturnItemsID = storeReturnItems.StoreReturnItemsID,
                ItemCategoryCode = storeReturnItems.ItemCategoryCode,
                ItemCode = storeReturnItems.ItemCode,
                UnitPrice = storeReturnItems.UnitPrice,
                Quantity = storeReturnItems.Quantity,
                StoreReturnID = storeReturnItems.StoreReturnID,

            });

            return Json(result);
        }


       // [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnItems_Create([DataSourceRequest]DataSourceRequest request, StoreReturnItems storeReturnItems, int id)
        {
            var record = db.StoreReturns.Find(id);
            var exist =
              db.StoreReturnItemss.Where(x => x.StoreReturnID == id)
                  .Where(x => x.ItemCode == storeReturnItems.ItemCode)
                  .ToList();
            if (exist.Any())
                ModelState.AddModelError("exist", "Item " + storeReturnItems.ItemCode + " already exists");

            if (record != null)
            {
                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(storeReturnItems.ItemCode, record.StoreCode);
                if (avgPrice == 0 && storeReturnItems.UnitPrice == 0)
                    ModelState.AddModelError("", "Please enter price,since there is no price set in the system.");
            }

            if (ModelState.IsValid)
            {
                var entity = new StoreReturnItems
                {
                    StoreReturnID = id,
                    ItemCategoryCode = storeReturnItems.ItemCategoryCode,
                    ItemCode = storeReturnItems.ItemCode,
                    UnitPrice = storeReturnItems.UnitPrice,
                    Quantity = storeReturnItems.Quantity,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };

                db.StoreReturnItemss.Add(entity);
                db.SaveChanges();
                storeReturnItems.StoreReturnItemsID = entity.StoreReturnItemsID;
                var storeReturn = db.StoreReturns.Find(id);
                //if (storeReturn != null)
                //    _distributionHelper.UpdateStoreReturnDistribution(storeReturn);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return item", "Success", User.Identity.Name, $"created store return item {entity.ItemCode} under record {entity.StoreReturnID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeReturnItems }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnItems_Update([DataSourceRequest]DataSourceRequest request, StoreReturnItems storeReturnItems)
        {
            var record = db.StoreReturns.Find(storeReturnItems.StoreReturnID);
            var exist =
             db.StoreReturnItemss.Where(x => x.StoreReturnID == storeReturnItems.StoreReturnID)
                    .Where(x => x.StoreReturnItemsID != storeReturnItems.StoreReturnItemsID)
                 .Where(x => x.ItemCode == storeReturnItems.ItemCode)
                 .ToList();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + storeReturnItems.ItemCode + " already exists");
            }
            if (record != null)
            {
                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(storeReturnItems.ItemCode, record.StoreCode);
                if (avgPrice == 0 && storeReturnItems.UnitPrice == 0)
                    ModelState.AddModelError("", "Please enter price,since there is no price set in the system.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnItems
                {
                    StoreReturnItemsID = storeReturnItems.StoreReturnItemsID,
                    StoreReturnID = storeReturnItems.StoreReturnID,
                    UnitPrice = storeReturnItems.UnitPrice,
                    ItemCategoryCode = storeReturnItems.ItemCategoryCode,
                    ItemCode = storeReturnItems.ItemCode,
                    Quantity = storeReturnItems.Quantity,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.StoreReturnItemss.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var storeReturn = db.StoreReturns.Find(storeReturnItems.StoreReturnID);
                //if (storeReturn != null)
                //    _distributionHelper.UpdateStoreReturnDistribution(storeReturn);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return item", "Success", User.Identity.Name, $"updated store return item {entity.ItemCode} under record {entity.StoreReturnID} ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { storeReturnItems }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnItems_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturnItems storeReturnItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnItems
                {
                    StoreReturnItemsID = storeReturnItems.StoreReturnItemsID,
                    StoreReturnID = storeReturnItems.StoreReturnID,
                    UnitPrice = storeReturnItems.UnitPrice,
                    ItemCategoryCode = storeReturnItems.ItemCategoryCode,
                    ItemCode = storeReturnItems.ItemCode,
                    Quantity = storeReturnItems.Quantity,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.StoreReturnItemss.Attach(entity);
                db.StoreReturnItemss.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store return item " + storeReturnItems.StoreReturnID + " " + storeReturnItems.InventoryItem.Name);
                #endregion
                var storeReturn = db.StoreReturns.Find(storeReturnItems.StoreReturnID);
                //if (storeReturn != null)
                //    _distributionHelper.UpdateStoreReturnDistribution(storeReturn);
            }

            return Json(new[] { storeReturnItems }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
