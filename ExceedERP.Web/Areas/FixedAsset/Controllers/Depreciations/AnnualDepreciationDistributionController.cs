using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
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
                AccountCode = annualDepreciationDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(annualDepreciationDistribution.AccountCode),
                Debit = annualDepreciationDistribution.Debit,
                Credit = annualDepreciationDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Create([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution issueDistribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationId = id,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.AnnualDepreciationDistributions.Add(entity);
                _db.SaveChanges();
                issueDistribution.AnnualDepreciationDistributionId = entity.AnnualDepreciationDistributionId;
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Update([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationDistributionId = issueDistribution.AnnualDepreciationDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.AnnualDepreciationDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualDepreciationDistributions_Destroy([DataSourceRequest]DataSourceRequest request, AnnualDepreciationDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualDepreciationDistribution
                {
                    AnnualDepreciationDistributionId = issueDistribution.AnnualDepreciationDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.AnnualDepreciationDistributions.Attach(entity);
                _db.AnnualDepreciationDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
