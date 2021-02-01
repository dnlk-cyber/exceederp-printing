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
    public class ApTravelExpenseDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelExpenseDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseDistribution> aptravelexpensedistributions = _db.ApTravelExpenseDistributions.Where(x=>x.ApTravelExpenseClaimId==id);
            DataSourceResult result = aptravelexpensedistributions.ToDataSourceResult(request, apTravelExpenseDistribution => new {
                ApTravelExpenseDistributionId = apTravelExpenseDistribution.ApTravelExpenseDistributionId,
                ApTravelExpenseClaimId = apTravelExpenseDistribution.ApTravelExpenseClaimId,
                AccountCode = apTravelExpenseDistribution.AccountCode,
               AccountDesc = _accountHelper.GetAccountDesc(apTravelExpenseDistribution.AccountCode),
                AdditionalDescription = apTravelExpenseDistribution.AdditionalDescription,
                Debit = apTravelExpenseDistribution.Debit,
                Credit = apTravelExpenseDistribution.Credit,
                OrgBranchName = apTravelExpenseDistribution.OrgBranchName,
                DateCreated = apTravelExpenseDistribution.DateCreated,
                LastModified = apTravelExpenseDistribution.LastModified,
                CreatedBy = apTravelExpenseDistribution.CreatedBy,
                ModifiedBy = apTravelExpenseDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution apTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseClaimId=id,
                    AccountCode = apTravelExpenseDistribution.AccountCode,
                   AccountDesc = _accountHelper.GetAccountDesc(apTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = apTravelExpenseDistribution.AdditionalDescription,
                    Debit = apTravelExpenseDistribution.Debit,
                    Credit = apTravelExpenseDistribution.Credit,
                    OrgBranchName = apTravelExpenseDistribution.OrgBranchName,
                    DateCreated = apTravelExpenseDistribution.DateCreated,
                    LastModified = apTravelExpenseDistribution.LastModified,
                    CreatedBy = apTravelExpenseDistribution.CreatedBy,
                    ModifiedBy = apTravelExpenseDistribution.ModifiedBy
                };

                _db.ApTravelExpenseDistributions.Add(entity);
                _db.SaveChanges();
                apTravelExpenseDistribution.ApTravelExpenseDistributionId = entity.ApTravelExpenseDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim distribution", "Success", User.Identity.Name, $"created expense claim distribution {entity.AccountCode} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution apTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseDistributionId = apTravelExpenseDistribution.ApTravelExpenseDistributionId,
                    ApTravelExpenseClaimId = apTravelExpenseDistribution.ApTravelExpenseClaimId,
                    AccountCode = apTravelExpenseDistribution.AccountCode,
                   AccountDesc = _accountHelper.GetAccountDesc(apTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = apTravelExpenseDistribution.AdditionalDescription,
                    Debit = apTravelExpenseDistribution.Debit,
                    Credit = apTravelExpenseDistribution.Credit,
                    OrgBranchName = apTravelExpenseDistribution.OrgBranchName,
                    DateCreated = apTravelExpenseDistribution.DateCreated,
                    LastModified = apTravelExpenseDistribution.LastModified,
                    CreatedBy = apTravelExpenseDistribution.CreatedBy,
                    ModifiedBy = apTravelExpenseDistribution.ModifiedBy
                };

                _db.ApTravelExpenseDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim distribution", "Success", User.Identity.Name, $"updated expense claim distribution {entity.AccountCode} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution apTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseDistributionId = apTravelExpenseDistribution.ApTravelExpenseDistributionId,
                    ApTravelExpenseClaimId = apTravelExpenseDistribution.ApTravelExpenseClaimId,
                    AccountCode = apTravelExpenseDistribution.AccountCode,
                   AccountDesc = _accountHelper.GetAccountDesc(apTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = apTravelExpenseDistribution.AdditionalDescription,
                    Debit = apTravelExpenseDistribution.Debit,
                    Credit = apTravelExpenseDistribution.Credit,
                    OrgBranchName = apTravelExpenseDistribution.OrgBranchName,
                    DateCreated = apTravelExpenseDistribution.DateCreated,
                    LastModified = apTravelExpenseDistribution.LastModified,
                    CreatedBy = apTravelExpenseDistribution.CreatedBy,
                    ModifiedBy = apTravelExpenseDistribution.ModifiedBy
                };

                _db.ApTravelExpenseDistributions.Attach(entity);
                _db.ApTravelExpenseDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
