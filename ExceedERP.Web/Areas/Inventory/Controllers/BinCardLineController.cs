using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

    public class BinCardLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyStoreKeeper)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyBincard)]
        public ActionResult BinCardLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BinCardLine> bincardlines = db.BinCardLines.Where(x=>x.StoreItemAssignmentID==id);

            string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();

            if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
            {
                bincardlines = bincardlines
                     .OrderBy(x => x.Date)
                      .ThenBy(x => x.BinCardLineID)
                      .AsQueryable();
            }
            else
            {
                bincardlines = bincardlines
                     .OrderBy(x => x.BinCardLineID)
                      .AsQueryable();
            }

            DataSourceResult result = bincardlines.ToDataSourceResult(request, binCardLine => new {
                BinCardLineID = binCardLine.BinCardLineID,
                StoreItemAssignmentID = binCardLine.StoreItemAssignmentID,
                Date = binCardLine.Date,
                Reference = binCardLine.Reference,
                ReferenceDoc = binCardLine.ReferenceDoc,
                ReceivedQTY = binCardLine.ReceivedQTY,
                ReceivedUPrice = binCardLine.ReceivedUPrice,
                ReceivedTPrice = binCardLine.ReceivedTPrice,
                IssuedQTY = binCardLine.IssuedQTY,
                IssuedUPrice = binCardLine.IssuedUPrice,
                IssuedTPrice = binCardLine.IssuedTPrice,
                BalanceQTY = binCardLine.BalanceQTY,
                BalanceUPrice = binCardLine.BalanceUPrice,
                BalanceTPrice = binCardLine.BalanceTPrice,
                
                GlFiscalYearId = binCardLine.GlFiscalYearId,
                GLPeriodId = binCardLine.GLPeriodId,
                TimeStamp = binCardLine.TimeStamp,
                IsVoid = binCardLine.IsVoid,
                IssuedTo = binCardLine.IssuedTo,
                SupplierName = binCardLine.SupplierName,

                StoreCode = binCardLine.StoreCode,
                TransactionType = binCardLine.TransactionType,
                StatusType = binCardLine.StatusType,
                TransactionOrder = binCardLine.TransactionOrder,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyBincard)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BinCardLines_Create([DataSourceRequest]DataSourceRequest request, BinCardLine binCardLine)
        {
            #region validation
            if (binCardLine.ReceivedQTY < 0)
                ModelState.AddModelError("invalid value", "Received Quanity should be above zero.");
            if (binCardLine.ReceivedUPrice < 0)
                ModelState.AddModelError("invalid value", "Received Unit Price should be above zero.");
            if (binCardLine.IssuedQTY < 0)
                ModelState.AddModelError("invalid value", "Issued Quanity should be above zero.");
            if (binCardLine.IssuedUPrice < 0)
                ModelState.AddModelError("invalid value", "Issued Unit Price should be above zero.");
            if (binCardLine.BalanceQTY < 0)
                ModelState.AddModelError("invalid value", "Balance Quanity should be above zero.");
            if (binCardLine.BalanceUPrice < 0)
                ModelState.AddModelError("invalid value", "Balance Unit Price should be above zero.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new BinCardLine
                {
                    Date = binCardLine.Date,
                    ReferenceDoc = binCardLine.ReferenceDoc,
                    ReceivedQTY = binCardLine.ReceivedQTY,
                    ReceivedUPrice = binCardLine.ReceivedUPrice,
                    ReceivedTPrice = binCardLine.ReceivedTPrice,
                    IssuedQTY = binCardLine.IssuedQTY,
                    IssuedUPrice = binCardLine.IssuedUPrice,
                    IssuedTPrice = binCardLine.IssuedTPrice,
                    BalanceQTY = binCardLine.BalanceQTY,
                    BalanceUPrice = binCardLine.BalanceUPrice,
                    BalanceTPrice = binCardLine.BalanceTPrice,
                    TransactionOrder = binCardLine.TransactionOrder,
                };

                //db.BinCardLines.Add(entity);
                //db.SaveChanges();
                //binCardLine.BinCardLineID = entity.BinCardLineID;
            }

            return Json(new[] { binCardLine }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyBincard)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BinCardLines_Update([DataSourceRequest]DataSourceRequest request, BinCardLine binCardLine)
        {
            #region validation
            if (binCardLine.ReceivedQTY < 0)
                ModelState.AddModelError("invalid value","Received Quanity should be above zero.");
            if (binCardLine.ReceivedUPrice < 0)
                ModelState.AddModelError("invalid value", "Received Unit Price should be above zero.");
            if (binCardLine.IssuedQTY < 0)
                ModelState.AddModelError("invalid value", "Issued Quanity should be above zero.");
            if (binCardLine.IssuedUPrice < 0)
                ModelState.AddModelError("invalid value", "Issued Unit Price should be above zero.");
            if(binCardLine.BalanceQTY < 0)
                ModelState.AddModelError("invalid value", "Balance Quanity should be above zero.");
            if (binCardLine.BalanceUPrice < 0)
                ModelState.AddModelError("invalid value", "Balance Unit Price should be above zero.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new BinCardLine
                {
                    BinCardLineID = binCardLine.BinCardLineID,
                    StoreItemAssignmentID = binCardLine.StoreItemAssignmentID,
                    Date = binCardLine.Date,
                    Reference = binCardLine.Reference,
                    ReferenceDoc = binCardLine.ReferenceDoc,
                    ReceivedQTY = binCardLine.ReceivedQTY,
                    ReceivedUPrice = binCardLine.ReceivedUPrice,
                    ReceivedTPrice = binCardLine.ReceivedQTY* binCardLine.ReceivedUPrice,
                    IssuedQTY = binCardLine.IssuedQTY,
                    IssuedUPrice = binCardLine.IssuedUPrice,
                    IssuedTPrice = binCardLine.IssuedQTY * binCardLine.IssuedUPrice,
                    BalanceQTY = binCardLine.BalanceQTY,
                    BalanceUPrice = binCardLine.BalanceUPrice,
                    BalanceTPrice = binCardLine.BalanceQTY* binCardLine.BalanceUPrice,

                    GlFiscalYearId = binCardLine.GlFiscalYearId,
                    GLPeriodId = binCardLine.GLPeriodId,
                    TimeStamp = binCardLine.TimeStamp,
                    IsVoid = binCardLine.IsVoid,
                    IssuedTo = binCardLine.IssuedTo,
                    SupplierName = binCardLine.SupplierName,
                    StoreCode = binCardLine.StoreCode,
                    TransactionType = binCardLine.TransactionType,
                    StatusType = binCardLine.StatusType,
                    TransactionOrder = binCardLine.TransactionOrder,
                };

                db.BinCardLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region  log
                UserHelper.OperationLog(Request.UserHostAddress, "BinCard Edit", "Success", User.Identity.Name, $"Bin Card updated item" +
                                                                                                                    $" {entity.StoreItemAssignment?.InventoryItem?.ItemCode} {entity.StoreItemAssignment?.InventoryItem?.Name} at ID {entity.BinCardLineID}-{entity.ReferenceDoc}",Core.Domain.Global.Modules.Inventory);
                #endregion
            }

            return Json(new[] { binCardLine }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyBincard)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BinCardLines_Destroy([DataSourceRequest]DataSourceRequest request, BinCardLine binCardLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BinCardLine
                {
                    BinCardLineID = binCardLine.BinCardLineID,
                    Date = binCardLine.Date,
                    ReferenceDoc = binCardLine.ReferenceDoc,
                    ReceivedQTY = binCardLine.ReceivedQTY,
                    ReceivedUPrice = binCardLine.ReceivedUPrice,
                    ReceivedTPrice = binCardLine.ReceivedTPrice,
                    IssuedQTY = binCardLine.IssuedQTY,
                    IssuedUPrice = binCardLine.IssuedUPrice,
                    IssuedTPrice = binCardLine.IssuedTPrice,
                    BalanceQTY = binCardLine.BalanceQTY,
                    BalanceUPrice = binCardLine.BalanceUPrice,
                    BalanceTPrice = binCardLine.BalanceTPrice,
                    TransactionOrder = binCardLine.TransactionOrder,
                };

                //db.BinCardLines.Attach(entity);
                //db.BinCardLines.Remove(entity);
                //db.SaveChanges();
                //#region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted bin card "+binCardLine.BinCardLineID);
                //#endregion
            }

            return Json(new[] { binCardLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
