using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{
  
    public class FixedAssetReceiveDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult FixedAssetReceiveDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReceiveDistribution> fixedAssetReceiveDistributions = db.FixedAssetReceiveDistributions.Where(gr => gr.FixedAssetReceiveId == id);
            DataSourceResult result = fixedAssetReceiveDistributions.ToDataSourceResult(request, fixedAssetReceiveDistribution => new
            {
                FixedAssetReceiveDistributionID = fixedAssetReceiveDistribution.FixedAssetReceiveDistributionID,
                FixedAssetReceiveId = fixedAssetReceiveDistribution.FixedAssetReceiveId,
                AccountCode = fixedAssetReceiveDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(fixedAssetReceiveDistribution.AccountCode),
                Debit = fixedAssetReceiveDistribution.Debit,
                Credit = fixedAssetReceiveDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveDistributions_Create([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveDistribution fixedAssetReceiveDistribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveDistribution
                {
                    FixedAssetReceiveId = id,
                    AccountCode = fixedAssetReceiveDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(fixedAssetReceiveDistribution.AccountCode),
                    Debit = fixedAssetReceiveDistribution.Debit,
                    Credit = fixedAssetReceiveDistribution.Credit
                };

                db.FixedAssetReceiveDistributions.Add(entity);
                db.SaveChanges();
                fixedAssetReceiveDistribution.FixedAssetReceiveDistributionID = entity.FixedAssetReceiveDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveDistribution", "Success", User.Identity.Name, $"created FixedAssetReceiveDistribution {entity.FixedAssetReceiveDistributionID} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { fixedAssetReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveDistributions_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveDistribution fixedAssetReceiveDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveDistribution
                {
                    FixedAssetReceiveDistributionID = fixedAssetReceiveDistribution.FixedAssetReceiveDistributionID,
                    FixedAssetReceiveId = fixedAssetReceiveDistribution.FixedAssetReceiveId,
                    AccountCode = fixedAssetReceiveDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(fixedAssetReceiveDistribution.AccountCode),
                    Debit = fixedAssetReceiveDistribution.Debit,
                    Credit = fixedAssetReceiveDistribution.Credit
                };

                db.FixedAssetReceiveDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveDistribution", "Success", User.Identity.Name, $"Update FixedAssetReceiveDistribution {entity.FixedAssetReceiveDistributionID} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveDistributions_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveDistribution fixedAssetReceiveDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveDistribution
                {
                    FixedAssetReceiveDistributionID = fixedAssetReceiveDistribution.FixedAssetReceiveDistributionID,
                    FixedAssetReceiveId = fixedAssetReceiveDistribution.FixedAssetReceiveId,
                    AccountCode = fixedAssetReceiveDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(fixedAssetReceiveDistribution.AccountCode),
                    Debit = fixedAssetReceiveDistribution.Debit,
                    Credit = fixedAssetReceiveDistribution.Credit
                };

                db.FixedAssetReceiveDistributions.Attach(entity);
                db.FixedAssetReceiveDistributions.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveDistribution", "Success", User.Identity.Name, $"created FixedAssetReceiveDistribution {fixedAssetReceiveDistribution.FixedAssetReceiveDistributionID} Tag {fixedAssetReceiveDistribution.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
