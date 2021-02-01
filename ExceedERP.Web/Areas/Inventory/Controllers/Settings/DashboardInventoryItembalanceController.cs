using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    public class DashboardInventoryItembalanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;

          
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult InventoryItembalances_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventoryItembalance> inventoryitembalances = db.InventoryItembalances;
            DataSourceResult result = inventoryitembalances.ToDataSourceResult(request, inventoryItembalance => new {
                InventoryItembalanceId = inventoryItembalance.InventoryItembalanceId,
                ItemCategoryCode = inventoryItembalance.ItemCategoryCode,
                ItemCode = inventoryItembalance.ItemCode,
                Balance = inventoryItembalance.Balance,
                LastUpdateAt = inventoryItembalance.LastUpdateAt,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryItembalances_Create([DataSourceRequest]DataSourceRequest request, InventoryItembalance inventoryItembalance)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventoryItembalance
                {
                    ItemCategoryCode = inventoryItembalance.ItemCategoryCode,
                    ItemCode = inventoryItembalance.ItemCode,
                };

                db.InventoryItembalances.Add(entity);
                db.SaveChanges();
                inventoryItembalance.InventoryItembalanceId = entity.InventoryItembalanceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory item dashboard", "Success", User.Identity.Name, $"created item {entity.ItemCode} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { inventoryItembalance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryItembalances_Update([DataSourceRequest]DataSourceRequest request, InventoryItembalance inventoryItembalance)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventoryItembalance
                {
                    ItemCategoryCode = inventoryItembalance.ItemCategoryCode,
                    ItemCode = inventoryItembalance.ItemCode,
                    InventoryItembalanceId = inventoryItembalance.InventoryItembalanceId,
                };

                db.InventoryItembalances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory item dashboard", "Success", User.Identity.Name, $"updated item {entity.ItemCode} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { inventoryItembalance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryItembalances_Destroy([DataSourceRequest]DataSourceRequest request, InventoryItembalance inventoryItembalance)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventoryItembalance
                {
                    ItemCategoryCode = inventoryItembalance.ItemCategoryCode,
                    ItemCode = inventoryItembalance.ItemCode,
                    InventoryItembalanceId = inventoryItembalance.InventoryItembalanceId,
                };

                db.InventoryItembalances.Attach(entity);
                db.InventoryItembalances.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item balance dashboard "+inventoryItembalance.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { inventoryItembalance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
