using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
    
    public class GoodSellingAndLendingDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private AccountHelper accountHelper = new AccountHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult GoodSellingAndLendingDistributions_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<GoodSellingAndLendingDistribution> goodsellingandlendingdistributions = db.GoodSellingAndLendingDistributions.Where(gsl => gsl.GoodSellingAndLendingID == id);
            DataSourceResult result = goodsellingandlendingdistributions.ToDataSourceResult(request, goodSellingAndLendingDistribution => new {
                GoodSellingAndLendingDistributionID = goodSellingAndLendingDistribution.GoodSellingAndLendingDistributionID,
                AdditionalDescription = goodSellingAndLendingDistribution.AdditionalDescription,
                AccountCode = goodSellingAndLendingDistribution.AccountCode,
                AccountDesc = accountHelper.GetAccountDesc(goodSellingAndLendingDistribution.AccountCode),
                Debit = goodSellingAndLendingDistribution.Debit,
                Credit = goodSellingAndLendingDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingDistributions_Create([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingDistribution goodSellingAndLendingDistribution , int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLendingDistribution
                {
                    GoodSellingAndLendingID = id,
                    AdditionalDescription = goodSellingAndLendingDistribution.AdditionalDescription,
                    AccountCode = goodSellingAndLendingDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(goodSellingAndLendingDistribution.AccountCode),
                    Debit = goodSellingAndLendingDistribution.Debit,
                    Credit = goodSellingAndLendingDistribution.Credit
                };

                db.GoodSellingAndLendingDistributions.Add(entity);
                db.SaveChanges();
                goodSellingAndLendingDistribution.GoodSellingAndLendingDistributionID = entity.GoodSellingAndLendingDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good selling", "Success", User.Identity.Name, $"created good selling  distribution {entity.AccountCode} under record {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodSellingAndLendingDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingDistributions_Update([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingDistribution goodSellingAndLendingDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLendingDistribution
                {
                    GoodSellingAndLendingDistributionID = goodSellingAndLendingDistribution.GoodSellingAndLendingDistributionID,
                    AdditionalDescription = goodSellingAndLendingDistribution.AdditionalDescription,
                    AccountCode = goodSellingAndLendingDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(goodSellingAndLendingDistribution.AccountCode),
                    Debit = goodSellingAndLendingDistribution.Debit,
                    Credit = goodSellingAndLendingDistribution.Credit
                };

                db.GoodSellingAndLendingDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good selling", "Success", User.Identity.Name, $"updated good selling  distribution {entity.AccountCode} under record {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodSellingAndLendingDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendingDistributions_Destroy([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingDistribution goodSellingAndLendingDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLendingDistribution
                {
                    GoodSellingAndLendingDistributionID = goodSellingAndLendingDistribution.GoodSellingAndLendingDistributionID,
                    AdditionalDescription = goodSellingAndLendingDistribution.AdditionalDescription,
                    AccountCode = goodSellingAndLendingDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(goodSellingAndLendingDistribution.AccountCode),
                    Debit = goodSellingAndLendingDistribution.Debit,
                    Credit = goodSellingAndLendingDistribution.Credit
                };

                db.GoodSellingAndLendingDistributions.Attach(entity);
                db.GoodSellingAndLendingDistributions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Inventory sales distribution " + goodSellingAndLendingDistribution.GoodSellingAndLendingID + " " + goodSellingAndLendingDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { goodSellingAndLendingDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
