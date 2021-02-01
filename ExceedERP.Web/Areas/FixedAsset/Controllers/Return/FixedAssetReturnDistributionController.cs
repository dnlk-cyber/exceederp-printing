using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.FixedAsset;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{
   

    public class FixedAssetReturnDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReturnDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReturnDistribution> fixedAssetReturnDistributions = _db.FixedAssetReturnDistributions.Where(ii => ii.FixedAssetReturnId == id);
            DataSourceResult result = fixedAssetReturnDistributions.ToDataSourceResult(request, fixedAssetReturnDistribution => new
            {
                FixedAssetReturnDistributionId = fixedAssetReturnDistribution.FixedAssetReturnDistributionId,
                AccountCode = fixedAssetReturnDistribution.AccountCode,
                AccountDesc = fixedAssetReturnDistribution.AccountDesc,
                Debit = fixedAssetReturnDistribution.Debit,
                Credit = fixedAssetReturnDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDistributions_Create([DataSourceRequest]DataSourceRequest request, FixedAssetReturnDistribution issueDistribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnDistribution
                {
                    FixedAssetReturnId = id,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.FixedAssetReturnDistributions.Add(entity);
                _db.SaveChanges();
                issueDistribution.FixedAssetReturnDistributionId = entity.FixedAssetReturnDistributionId;
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDistributions_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReturnDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnDistribution
                {
                    FixedAssetReturnDistributionId = issueDistribution.FixedAssetReturnDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.FixedAssetReturnDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDistributions_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReturnDistribution issueDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnDistribution
                {
                    FixedAssetReturnDistributionId = issueDistribution.FixedAssetReturnDistributionId,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                _db.FixedAssetReturnDistributions.Attach(entity);
                _db.FixedAssetReturnDistributions.Remove(entity);
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
