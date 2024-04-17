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
    
    public class StockAdjustmentDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser, InventoryConstants.PropertyStockAdjustmentApprover)]
        public ActionResult StockAdjustmentDistributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StockAdjustmentDistribution> stockadjustmentdistributions = db.StockAdjustmentDistributions;
            DataSourceResult result = stockadjustmentdistributions.ToDataSourceResult(request, stockAdjustmentDistribution => new {
                StockAdjustmentDistributionID = stockAdjustmentDistribution.StockAdjustmentDistributionID,
                AccountCode = stockAdjustmentDistribution.AccountCode,
                AccountDesc = stockAdjustmentDistribution.AccountDesc,
                Debit = stockAdjustmentDistribution.Debit,
                Credit = stockAdjustmentDistribution.Credit
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentDistributions_Create([DataSourceRequest]DataSourceRequest request, StockAdjustmentDistribution stockAdjustmentDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentDistribution
                {
                    AccountCode = stockAdjustmentDistribution.AccountCode,
                    AccountDesc = stockAdjustmentDistribution.AccountDesc,
                    Debit = stockAdjustmentDistribution.Debit,
                    Credit = stockAdjustmentDistribution.Credit
                };

                db.StockAdjustmentDistributions.Add(entity);
                db.SaveChanges();
                stockAdjustmentDistribution.StockAdjustmentDistributionID = entity.StockAdjustmentDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment distribution", "Success", User.Identity.Name, $"created stock adjustment distribution {entity.AccountCode}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustmentDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentDistributions_Update([DataSourceRequest]DataSourceRequest request, StockAdjustmentDistribution stockAdjustmentDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentDistribution
                {
                    StockAdjustmentDistributionID = stockAdjustmentDistribution.StockAdjustmentDistributionID,
                    AccountCode = stockAdjustmentDistribution.AccountCode,
                    AccountDesc = stockAdjustmentDistribution.AccountDesc,
                    Debit = stockAdjustmentDistribution.Debit,
                    Credit = stockAdjustmentDistribution.Credit
                };

                db.StockAdjustmentDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment distribution", "Success", User.Identity.Name, $"updated stock adjustment distribution {entity.AccountCode}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustmentDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStockAdjustmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustmentDistributions_Destroy([DataSourceRequest]DataSourceRequest request, StockAdjustmentDistribution stockAdjustmentDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustmentDistribution
                {
                    StockAdjustmentDistributionID = stockAdjustmentDistribution.StockAdjustmentDistributionID,
                    AccountCode = stockAdjustmentDistribution.AccountCode,
                    AccountDesc = stockAdjustmentDistribution.AccountDesc,
                    Debit = stockAdjustmentDistribution.Debit,
                    Credit = stockAdjustmentDistribution.Credit
                };

                db.StockAdjustmentDistributions.Attach(entity);
                db.StockAdjustmentDistributions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Stock adjustment distribution " + stockAdjustmentDistribution.StockAdjustmentID + " " + stockAdjustmentDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { stockAdjustmentDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
