using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{


    public class IfrsDepreciationDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult IfrsDepreciationDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IfrsDepreciationDistribution> IfrsDepreciationDistributions = _db.IfrsDepreciationDistributions.Where(ii => ii.IfrsDepreciationId == id);
            DataSourceResult result = IfrsDepreciationDistributions.ToDataSourceResult(request, IfrsDepreciationDistribution => new
            {
                IfrsDepreciationDistributionId = IfrsDepreciationDistribution.IfrsDepreciationDistributionId,
                IfrsDepreciationId = IfrsDepreciationDistribution.IfrsDepreciationId,
                AccountCode = IfrsDepreciationDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(IfrsDepreciationDistribution.AccountCode),
                Debit = IfrsDepreciationDistribution.Debit,
                Credit = IfrsDepreciationDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationDistributions_Create([DataSourceRequest]DataSourceRequest request, IfrsDepreciationDistribution IfrsDist, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsDepreciationDistribution
                {
                    IfrsDepreciationId = id,
                    AccountCode = IfrsDist.AccountCode,
                    AccountDesc = IfrsDist.AccountDesc,
                    Debit = IfrsDist.Debit,
                    Credit = IfrsDist.Credit
                };

                _db.IfrsDepreciationDistributions.Add(entity);
                _db.SaveChanges();
                IfrsDist.IfrsDepreciationDistributionId = entity.IfrsDepreciationDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " Ifrs Depreciations", "Success", User.Identity.Name, $"created Ifrs depreciations {entity.IfrsDepreciationId} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { IfrsDist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationDistributions_Update([DataSourceRequest]DataSourceRequest request, IfrsDepreciationDistribution IfrsDist)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsDepreciationDistribution
                {
                    IfrsDepreciationDistributionId = IfrsDist.IfrsDepreciationDistributionId,
                    IfrsDepreciationId = IfrsDist.IfrsDepreciationId,                    
                    AccountCode = IfrsDist.AccountCode,
                    AccountDesc = IfrsDist.AccountDesc,
                    Debit = IfrsDist.Debit,
                    Credit = IfrsDist.Credit
                };

                _db.IfrsDepreciationDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " Ifrs Depreciations", "Success", User.Identity.Name, $"update Ifrs depreciations Distributions{entity.IfrsDepreciationId} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { IfrsDist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationDistributions_Destroy([DataSourceRequest]DataSourceRequest request, IfrsDepreciationDistribution IfrsDist)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsDepreciationDistribution
                {
                    IfrsDepreciationDistributionId = IfrsDist.IfrsDepreciationDistributionId,
                    IfrsDepreciationId = IfrsDist.IfrsDepreciationId,
                    AccountCode = IfrsDist.AccountCode,
                    AccountDesc = IfrsDist.AccountDesc,
                    Debit = IfrsDist.Debit,
                    Credit = IfrsDist.Credit
                };

                _db.IfrsDepreciationDistributions.Attach(entity);
                _db.IfrsDepreciationDistributions.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Ifrs Depreciation Distributions {IfrsDist.IfrsDepreciationDistributionId } and Tag {IfrsDist.AccountCode}");
                #endregion
            }

            return Json(new[] { IfrsDist }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
