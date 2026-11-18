using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
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

        [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult StockAdjustmentItemss_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StockAdjustmentItems> stockadjustmentItemss = db.StockAdjustmentItemss.Where(adj => adj.StockAdjustmentID == id);
            DataSourceResult result = stockadjustmentItemss.ToDataSourceResult(request, stockAdjustmentItems => new {
                StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                StockAdjustmentID = stockAdjustmentItems.StockAdjustmentID,
                ItemCategoryCode = stockAdjustmentItems.ItemCategoryCode,
                ItemCode = stockAdjustmentItems.ItemCode,
                BinCardBalance = stockAdjustmentItems.BinCardBalance,
                StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                AdjustedPrice = stockAdjustmentItems.AdjustedPrice,
                AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity  ,
                PriceAdjustmentType = stockAdjustmentItems.PriceAdjustmentType
                 
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Create([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems , int id )
            {
            var stockAdjustment = db.StockAdjustments.Find(id);
            if (stockAdjustment == null)
            {
                ModelState.AddModelError("record", "Stock adjustment record was not found.");
            }
            else
            {
                stockAdjustmentItems.StockAdjustmentID = id;
            }

            var exist = db.StockAdjustmentItemss
              .Where(x => x.StockAdjustmentID == id)
             .Where(x => x.ItemCode == stockAdjustmentItems.ItemCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + stockAdjustmentItems.ItemCode + " already exists");
            }

            var inventoryItem = db.InventoryItems.Find(stockAdjustmentItems.ItemCode);
            if (inventoryItem == null)
            {
                ModelState.AddModelError("item", "Select a valid item.");
            }
            else if (string.IsNullOrWhiteSpace(stockAdjustmentItems.ItemCategoryCode))
            {
                stockAdjustmentItems.ItemCategoryCode = inventoryItem.ItemCategoryCode;
            }

            var storeItem = stockAdjustment == null
                ? null
                : db.StoreItemAssignments.FirstOrDefault(it => it.ItemCode == stockAdjustmentItems.ItemCode && it.StoreCode == stockAdjustment.StoreCode);
            if (stockAdjustment != null && storeItem == null)
            {
                ModelState.AddModelError("store", "Item " + stockAdjustmentItems.ItemCode + " is not available in store " + stockAdjustment.StoreCode + ".");
            }
        
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentItems
                {
                    StockAdjustmentID = id,
                    ItemCategoryCode = stockAdjustmentItems.ItemCategoryCode,
                    ItemCode = stockAdjustmentItems.ItemCode,
                    BinCardBalance = storeItem.Balance,
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
                stockAdjustmentItems.BinCardBalance = entity.BinCardBalance;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment item", "Success", User.Identity.Name, $"created stock adjustment item {entity.ItemCode} under record {entity.StockAdjustmentID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustmentItems }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Update([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems, int id)
        {
            var stockAdjustment = db.StockAdjustments.Find(id);
            if (stockAdjustment == null)
            {
                ModelState.AddModelError("record", "Stock adjustment record was not found.");
            }
            else
            {
                stockAdjustmentItems.StockAdjustmentID = id;
            }

            var exist = db.StockAdjustmentItemss.Where(x => x.StockAdjustmentID == id)
             .Where(x => x.StockAdjustmentItemsID != stockAdjustmentItems.StockAdjustmentItemsID)
             .Where(x => x.ItemCode == stockAdjustmentItems.ItemCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + stockAdjustmentItems.ItemCode + " already exists");
            }

            var inventoryItem = db.InventoryItems.Find(stockAdjustmentItems.ItemCode);
            if (inventoryItem == null)
            {
                ModelState.AddModelError("item", "Select a valid item.");
            }
            else if (string.IsNullOrWhiteSpace(stockAdjustmentItems.ItemCategoryCode))
            {
                stockAdjustmentItems.ItemCategoryCode = inventoryItem.ItemCategoryCode;
            }

            var existing = db.StockAdjustmentItemss.Find(stockAdjustmentItems.StockAdjustmentItemsID);
            var storeItem = stockAdjustment == null
                ? null
                : db.StoreItemAssignments.FirstOrDefault(it => it.ItemCode == stockAdjustmentItems.ItemCode && it.StoreCode == stockAdjustment.StoreCode);
            if (stockAdjustment != null && storeItem == null)
            {
                ModelState.AddModelError("store", "Item " + stockAdjustmentItems.ItemCode + " is not available in store " + stockAdjustment.StoreCode + ".");
            }
           
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentItems
                {
                    StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                    StockAdjustmentID = id,
                    ItemCategoryCode = stockAdjustmentItems.ItemCategoryCode,
                    ItemCode = stockAdjustmentItems.ItemCode,
                    BinCardBalance = storeItem.Balance,
                    StockAdjustmentType = stockAdjustmentItems.StockAdjustmentType,
                    AdjustedQuantity = stockAdjustmentItems.AdjustedQuantity ,
                    AdjustedPrice = stockAdjustmentItems.AdjustedPrice ,
                    PriceAdjustmentType = stockAdjustmentItems.PriceAdjustmentType,
                    CreatedBy = existing != null ? existing.CreatedBy : stockAdjustmentItems.CreatedBy,
                    DateCreated = existing != null ? existing.DateCreated : stockAdjustmentItems.DateCreated,
                    UOM = existing != null ? existing.UOM : stockAdjustmentItems.UOM,
                    OrgBranchName = existing != null ? existing.OrgBranchName : stockAdjustmentItems.OrgBranchName,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                db.StockAdjustmentItemss.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                stockAdjustmentItems.BinCardBalance = entity.BinCardBalance;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment item", "Success", User.Identity.Name, $"updated stock adjustment item {entity.ItemCode} under record {entity.StockAdjustmentID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustmentItems }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentItemss_Destroy([DataSourceRequest]DataSourceRequest request, StockAdjustmentItems stockAdjustmentItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentItems
                {
                    StockAdjustmentItemsID = stockAdjustmentItems.StockAdjustmentItemsID,
                    ItemCategoryCode = stockAdjustmentItems.ItemCategoryCode,
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
