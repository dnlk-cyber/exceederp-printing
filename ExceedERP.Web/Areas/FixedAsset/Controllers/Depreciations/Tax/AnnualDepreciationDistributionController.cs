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


    public class AnnualDepreciationDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AnnualDepreciationDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AnnualDepreciationDistribution> annualDepreciationDistributions = _db.AnnualDepreciationDistributions.Where(ii => ii.AnnualDepreciationId == id);
            DataSourceResult result = annualDepreciationDistributions.ToDataSourceResult(request, annualDepreciationDistribution => new
            {
                AnnualDepreciationDistributionId = annualDepreciationDistribution.AnnualDepreciationDistributionId,
                AnnualDepreciationId = annualDepreciationDistribution.AnnualDepreciationId,
                AccountCode = annualDepreciationDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(annualDepreciationDistribution.AccountCode),
                Debit = annualDepreciationDistribution.Debit,
                Credit = annualDepreciationDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Create([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution annualDist, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationId = id,
                    AccountCode = annualDist.AccountCode,
                    AccountDesc = annualDist.AccountDesc,
                    Debit = annualDist.Debit,
                    Credit = annualDist.Credit
                };

                _db.AnnualDepreciationDistributions.Add(entity);
                _db.SaveChanges();
                annualDist.AnnualDepreciationDistributionId = entity.AnnualDepreciationDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " Annual Depreciations", "Success", User.Identity.Name, $"created annual depreciations {entity.AnnualDepreciationId} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { annualDist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Update([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution annualDist)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationDistributionId = annualDist.AnnualDepreciationDistributionId,
                    AnnualDepreciationId = annualDist.AnnualDepreciationId,                    
                    AccountCode = annualDist.AccountCode,
                    AccountDesc = annualDist.AccountDesc,
                    Debit = annualDist.Debit,
                    Credit = annualDist.Credit
                };

                _db.AnnualDepreciationDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " Annual Depreciations", "Success", User.Identity.Name, $"update annual depreciations Distributions{entity.AnnualDepreciationId} Tag {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { annualDist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Destroy([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution annualDist)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationDistributionId = annualDist.AnnualDepreciationDistributionId,
                    AnnualDepreciationId = annualDist.AnnualDepreciationId,
                    AccountCode = annualDist.AccountCode,
                    AccountDesc = annualDist.AccountDesc,
                    Debit = annualDist.Debit,
                    Credit = annualDist.Credit
                };

                _db.AnnualDepreciationDistributions.Attach(entity);
                _db.AnnualDepreciationDistributions.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Annual Depreciation Distributions {annualDist.AnnualDepreciationDistributionId } and Tag {annualDist.AccountCode}");
                #endregion
            }

            return Json(new[] { annualDist }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
