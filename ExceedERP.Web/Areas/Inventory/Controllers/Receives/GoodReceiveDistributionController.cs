using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{

    public class GoodReceiveDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private AccountHelper accountHelper = new AccountHelper();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer, InventoryConstants.PropertyInspector)]
        public ActionResult GoodReceiveDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GoodReceiveDistribution> goodreceivedistributions = db.GoodReceiveDistributions.Where(gr => gr.GoodReceiveId == id);
            DataSourceResult result = goodreceivedistributions.ToDataSourceResult(request, goodReceiveDistribution => new
            {
                GoodReceiveId = goodReceiveDistribution.GoodReceiveId,
                GoodReceiveDistributionID = goodReceiveDistribution.GoodReceiveDistributionID,
                GoodReceiveItemIDTrackBack = goodReceiveDistribution.GoodReceiveItemIDTrackBack,
                AccountCode = goodReceiveDistribution.AccountCode,
                AccountDesc = goodReceiveDistribution.AccountDesc,
                AdditionalDescription = goodReceiveDistribution.AdditionalDescription,
                Debit = goodReceiveDistribution.Debit,
                Credit = goodReceiveDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveDistributions_Create([DataSourceRequest]DataSourceRequest request, GoodReceiveDistribution goodReceiveDistribution, int id)
        {
            #region validation

            var record = db.GoodReceives.Find(id);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = db.GoodReceiveDistributions.Where(x => x.Description == goodReceiveDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveDistribution
                {
                    GoodReceiveId = id,
                    GoodReceiveItemIDTrackBack = goodReceiveDistribution.GoodReceiveItemIDTrackBack,
                    AccountCode = goodReceiveDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(goodReceiveDistribution.AccountCode),
                    AdditionalDescription = goodReceiveDistribution.AdditionalDescription,
                    Debit = goodReceiveDistribution.Debit,
                    Credit = goodReceiveDistribution.Credit
                };

                db.GoodReceiveDistributions.Add(entity);
                db.SaveChanges();
                goodReceiveDistribution.GoodReceiveDistributionID = entity.GoodReceiveDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive distribution", "Success", User.Identity.Name, $"created good receive  distribution {entity.AccountCode} under record {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult GoodReceiveDistributions_Update([DataSourceRequest]DataSourceRequest request, GoodReceiveDistribution goodReceiveDistribution)
        {
            #region validation

            var record = db.GoodReceives.Find(goodReceiveDistribution.GoodReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = db.GoodReceiveDistributions.Where(x => x.GoodReceiveDistributionID != goodReceiveDistribution.GoodReceiveDistributionID).Where(x => x.Description == goodReceiveDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveDistribution
                {
                    GoodReceiveId = goodReceiveDistribution.GoodReceiveId,
                    GoodReceiveDistributionID = goodReceiveDistribution.GoodReceiveDistributionID,
                    GoodReceiveItemIDTrackBack = goodReceiveDistribution.GoodReceiveItemIDTrackBack,
                    AccountCode = goodReceiveDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(goodReceiveDistribution.AccountCode),
                    AdditionalDescription = goodReceiveDistribution.AdditionalDescription,
                    Debit = goodReceiveDistribution.Debit,
                    Credit = goodReceiveDistribution.Credit,

                };

                db.GoodReceiveDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive distribution", "Success", User.Identity.Name, $"updated good receive  distribution {entity.AccountCode} under record {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult GoodReceiveDistributions_Destroy([DataSourceRequest]DataSourceRequest request, GoodReceiveDistribution goodReceiveDistribution)
        {
            var record = db.GoodReceives.Find(goodReceiveDistribution.GoodReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveDistribution
                {
                    GoodReceiveId = goodReceiveDistribution.GoodReceiveId,
                    GoodReceiveDistributionID = goodReceiveDistribution.GoodReceiveDistributionID,
                    GoodReceiveItemIDTrackBack = goodReceiveDistribution.GoodReceiveItemIDTrackBack,
                    AccountCode = goodReceiveDistribution.AccountCode,
                    AccountDesc = goodReceiveDistribution.AccountDesc,
                    AdditionalDescription = goodReceiveDistribution.AdditionalDescription,
                    Debit = goodReceiveDistribution.Debit,
                    Credit = goodReceiveDistribution.Credit
                };

                db.GoodReceiveDistributions.Attach(entity);
                db.GoodReceiveDistributions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive distribution " + goodReceiveDistribution.GoodReceiveId + " " + goodReceiveDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { goodReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
