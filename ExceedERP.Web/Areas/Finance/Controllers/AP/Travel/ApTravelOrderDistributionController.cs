using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelOrderDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelOrderDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderDistribution> aptravelorderdistributions = _db.ApTravelOrderDistributions.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = aptravelorderdistributions.ToDataSourceResult(request, apTravelOrderDistribution => new {
                ApTravelOrderDistributionId = apTravelOrderDistribution.ApTravelOrderDistributionId,
                ApTravelOrderId = apTravelOrderDistribution.ApTravelOrderId,
                AccountCode = apTravelOrderDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(apTravelOrderDistribution.AccountCode),
                AdditionalDescription = apTravelOrderDistribution.AdditionalDescription,
                Debit = apTravelOrderDistribution.Debit,
                Credit = apTravelOrderDistribution.Credit,
                OrgBranchName = apTravelOrderDistribution.OrgBranchName,
                DateCreated = apTravelOrderDistribution.DateCreated,
                LastModified = apTravelOrderDistribution.LastModified,
                CreatedBy = apTravelOrderDistribution.CreatedBy,
                ModifiedBy = apTravelOrderDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderDistribution apTravelOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDistribution
                {
                    ApTravelOrderId = id,
                    AccountCode = apTravelOrderDistribution.AccountCode,
                     AccountDesc = _accountHelper.GetAccountDesc(apTravelOrderDistribution.AccountCode),
                    AdditionalDescription = apTravelOrderDistribution.AdditionalDescription,
                    Debit = apTravelOrderDistribution.Debit,
                    Credit = apTravelOrderDistribution.Credit,
                    OrgBranchName = apTravelOrderDistribution.OrgBranchName,
                    DateCreated = apTravelOrderDistribution.DateCreated,
                    LastModified = apTravelOrderDistribution.LastModified,
                    CreatedBy = apTravelOrderDistribution.CreatedBy,
                    ModifiedBy = apTravelOrderDistribution.ModifiedBy
                };

                _db.ApTravelOrderDistributions.Add(entity);
                _db.SaveChanges();
                apTravelOrderDistribution.ApTravelOrderDistributionId = entity.ApTravelOrderDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order distribution", "Success", User.Identity.Name, $"created travel order distribution {entity.AccountCode} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDistributions_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderDistribution apTravelOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDistribution
                {
                    ApTravelOrderDistributionId = apTravelOrderDistribution.ApTravelOrderDistributionId,
                    ApTravelOrderId = apTravelOrderDistribution.ApTravelOrderId,
                    AccountCode = apTravelOrderDistribution.AccountCode,
                     AccountDesc = _accountHelper.GetAccountDesc(apTravelOrderDistribution.AccountCode),
                    AdditionalDescription = apTravelOrderDistribution.AdditionalDescription,
                    Debit = apTravelOrderDistribution.Debit,
                    Credit = apTravelOrderDistribution.Credit,
                    OrgBranchName = apTravelOrderDistribution.OrgBranchName,
                    DateCreated = apTravelOrderDistribution.DateCreated,
                    LastModified = apTravelOrderDistribution.LastModified,
                    CreatedBy = apTravelOrderDistribution.CreatedBy,
                    ModifiedBy = apTravelOrderDistribution.ModifiedBy
                };

                _db.ApTravelOrderDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderDistribution apTravelOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDistribution
                {
                    ApTravelOrderDistributionId = apTravelOrderDistribution.ApTravelOrderDistributionId,
                    ApTravelOrderId = apTravelOrderDistribution.ApTravelOrderId,
                    AccountCode = apTravelOrderDistribution.AccountCode,
                     AccountDesc = _accountHelper.GetAccountDesc(apTravelOrderDistribution.AccountCode),
                    AdditionalDescription = apTravelOrderDistribution.AdditionalDescription,
                    Debit = apTravelOrderDistribution.Debit,
                    Credit = apTravelOrderDistribution.Credit,
                    OrgBranchName = apTravelOrderDistribution.OrgBranchName,
                    DateCreated = apTravelOrderDistribution.DateCreated,
                    LastModified = apTravelOrderDistribution.LastModified,
                    CreatedBy = apTravelOrderDistribution.CreatedBy,
                    ModifiedBy = apTravelOrderDistribution.ModifiedBy
                };

                _db.ApTravelOrderDistributions.Attach(entity);
                _db.ApTravelOrderDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
