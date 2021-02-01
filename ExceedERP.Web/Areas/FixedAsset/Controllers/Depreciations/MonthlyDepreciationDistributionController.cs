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
   

    public class MonthlyDepreciationDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MonthlyDepreciationDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<MonthlyDepreciationDistribution> monthlyDepreciationDistributions = _db.MonthlyDepreciationDistributions.Where(ii => ii.MonthlyDepreciationId == id);
            DataSourceResult result = monthlyDepreciationDistributions.ToDataSourceResult(request, monthlyDepreciationDistribution => new
            {
                MonthlyDepreciationDistributionId = monthlyDepreciationDistribution.MonthlyDepreciationDistributionId,
                AccountCode = monthlyDepreciationDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(monthlyDepreciationDistribution.AccountCode),
                Debit = monthlyDepreciationDistribution.Debit,
                Credit = monthlyDepreciationDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationDistributions_Create([DataSourceRequest]DataSourceRequest request, MonthlyDepreciationDistribution issueDistribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyDepreciationDistribution
                {
                    MonthlyDepreciationId = id,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.MonthlyDepreciationDistributions.Add(entity);
                _db.SaveChanges();
                issueDistribution.MonthlyDepreciationDistributionId = entity.MonthlyDepreciationDistributionId;
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationDistributions_Update([DataSourceRequest]DataSourceRequest request, MonthlyDepreciationDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyDepreciationDistribution
                {
                    MonthlyDepreciationDistributionId = issueDistribution.MonthlyDepreciationDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.MonthlyDepreciationDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationDistributions_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyDepreciationDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyDepreciationDistribution
                {
                    MonthlyDepreciationDistributionId = issueDistribution.MonthlyDepreciationDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.MonthlyDepreciationDistributions.Attach(entity);
                _db.MonthlyDepreciationDistributions.Remove(entity);
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
