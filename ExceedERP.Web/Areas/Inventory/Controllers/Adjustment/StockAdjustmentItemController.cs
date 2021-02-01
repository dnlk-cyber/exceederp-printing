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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Adjustment
{
   
    public class StockAdjustmentItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StockAdjustmentItemss_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StockAdjustmentItems> stockadjustmentItemss = db.StockAdjustmentItemss.Where(adj => adj.StockAdjustmentID == id);
            DataSourceResult result = stockadjustmentItemss.ToDataSourceResult(request, stockAdjustmentItems => new {
                StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                StockAdjustmentID = stockAdjustmentItems.StockAdjustmentID,
                ItemCode = stockAdjustmentItems.ItemCode,
                BinCardBalance = stockAdjustmentItems.BinCardBalance,
                StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                AdjustedPrice = stockAdjustmentItems.AdjustedPrice,
                AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity  ,
                PriceAdjustmentType = stockAdjustmentItems.PriceAdjustmentType
                 
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Create([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems , int id )
            {
            var stockAdjustment = db.StockAdjustments.Find(id);

            var exist = db.StockAdjustmentItemss.Where(x => x.StockAdjustmentID == stockAdjustmentItems.StockAdjustmentID)
             .Where(x => x.ItemCode == stockAdjustmentItems.ItemCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + stockAdjustmentItems.ItemCode + " already exists");
            }

            var item = db.StoreItemAssignments.Where(it => it.ItemCode  == stockAdjustmentItems.ItemCode).FirstOrDefault(x => x.StoreCode  == stockAdjustment.StoreCode);
        
            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new StockAdjustmentItems
                    {
                        StockAdjustmentID = id,
                        ItemCode = stockAdjustmentItems.ItemCode,
                        BinCardBalance = item.Balance,
                        StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                        AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity,
                        AdjustedPrice = stockAdjustmentItems.AdjustedPrice,
                        PriceAdjustmentType = stockAdjustmentItems.PriceAdjustmentType,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                     
                    };

                    db.StockAdjustmentItemss.Add(entity);
                    db.SaveChanges();
                    stockAdjustmentItems.StockAdjustmentItemsID = entity.StockAdjustmentItemsID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment item", "Success", User.Identity.Name, $"created stock adjustment item {entity.ItemCode} under record {entity.StockAdjustmentID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { stockAdjustmentItems }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Update([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems, int id)
        {
            var stockAdjustment = db.StockAdjustments.Find(id);
            var exist = db.StockAdjustmentItemss.Where(x => x.StockAdjustmentID == stockAdjustmentItems.StockAdjustmentID)
             .Where(x => x.StockAdjustmentItemsID != stockAdjustmentItems.StockAdjustmentItemsID)
             .Where(x => x.ItemCode == stockAdjustmentItems.ItemCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + stockAdjustmentItems.ItemCode + " already exists");
            }
            var item = db.StoreItemAssignments.FirstOrDefault(it => it.ItemCode == stockAdjustmentItems.ItemCode && it.StoreCode == stockAdjustment.StoreCode);
           
            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new StockAdjustmentItems
                    {
                        StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                        StockAdjustmentID = stockAdjustmentItems.StockAdjustmentID,
                        ItemCode = stockAdjustmentItems.ItemCode,
                        BinCardBalance = item.Balance,
                        StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                        AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity ,
                        AdjustedPrice = stockAdjustmentItems.AdjustedPrice ,
                        PriceAdjustmentType = stockAdjustmentItems.PriceAdjustmentType,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.StockAdjustmentItemss.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment item", "Success", User.Identity.Name, $"updated stock adjustment item {entity.ItemCode} under record {entity.StockAdjustmentID} ", Modules.Inventory);
                    #endregion
                }

            }

            return Json(new[] { stockAdjustmentItems }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Destroy([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentItems
                {
                    StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                    ItemCode = stockAdjustmentItems.ItemCode,
                    BinCardBalance = stockAdjustmentItems.BinCardBalance,
                    StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                    AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity
                
                };

                db.StockAdjustmentItemss.Attach(entity);
                db.StockAdjustmentItemss.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted stock adjustment item " + stockAdjustmentItems.StockAdjustmentID + " " + stockAdjustmentItems.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { stockAdjustmentItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
