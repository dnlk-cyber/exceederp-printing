using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PettyCash
{
    public class ApPettyCashDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper helper = new AccountHelper();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApPettyCashDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPettyCashDistribution> appettycashdistributions = _db.ApPettyCashDistributions.Where(x=>x.ApPettyCashId==id);
            DataSourceResult result = appettycashdistributions.ToDataSourceResult(request, apPettyCashDistribution => new {
                ApPettyCashDistributionId = apPettyCashDistribution.ApPettyCashDistributionId,
                ApPettyCashId = apPettyCashDistribution.ApPettyCashId,
                AccountCode = apPettyCashDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(apPettyCashDistribution.AccountCode),
                AdditionalDescription = apPettyCashDistribution.AdditionalDescription,
                Debit = apPettyCashDistribution.Debit,
                Credit = apPettyCashDistribution.Credit,
                OrgBranchName = apPettyCashDistribution.OrgBranchName,
                DateCreated = apPettyCashDistribution.DateCreated,
                LastModified = apPettyCashDistribution.LastModified,
                CreatedBy = apPettyCashDistribution.CreatedBy,
                ModifiedBy = apPettyCashDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPettyCashDistribution apPettyCashDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashDistribution
                {
                    ApPettyCashId =id,
                    AccountCode = apPettyCashDistribution.AccountCode,
                    AccountDesc = helper.GetAccountDesc(apPettyCashDistribution.AccountCode),
                    AdditionalDescription = apPettyCashDistribution.AdditionalDescription,
                    Debit = apPettyCashDistribution.Debit,
                    Credit = apPettyCashDistribution.Credit,
                    OrgBranchName = apPettyCashDistribution.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = apPettyCashDistribution.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apPettyCashDistribution.ModifiedBy
                };

                _db.ApPettyCashDistributions.Add(entity);
                _db.SaveChanges();
                apPettyCashDistribution.ApPettyCashDistributionId = entity.ApPettyCashDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash distribution", "Success", User.Identity.Name, $"created petty cash distribution {entity.AccountCode} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashDistributions_Update([DataSourceRequest]DataSourceRequest request, ApPettyCashDistribution apPettyCashDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashDistribution
                {
                    ApPettyCashId = apPettyCashDistribution.ApPettyCashId,
                    ApPettyCashDistributionId = apPettyCashDistribution.ApPettyCashDistributionId,
                    AccountCode = apPettyCashDistribution.AccountCode,
                    AccountDesc = helper.GetAccountDesc(apPettyCashDistribution.AccountCode),
                    AdditionalDescription = apPettyCashDistribution.AdditionalDescription,
                    Debit = apPettyCashDistribution.Debit,
                    Credit = apPettyCashDistribution.Credit,
                    OrgBranchName = apPettyCashDistribution.OrgBranchName,
                    DateCreated = apPettyCashDistribution.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apPettyCashDistribution.CreatedBy,
                    ModifiedBy =User.Identity.Name
                };

                _db.ApPettyCashDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash distribution", "Success", User.Identity.Name, $"updated petty cash distribution {entity.AccountCode} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApPettyCashDistribution apPettyCashDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashDistribution
                {
                    ApPettyCashDistributionId = apPettyCashDistribution.ApPettyCashDistributionId,
                    AccountCode = apPettyCashDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(apPettyCashDistribution.AccountCode),
                    AdditionalDescription = apPettyCashDistribution.AdditionalDescription,
                    Debit = apPettyCashDistribution.Debit,
                    Credit = apPettyCashDistribution.Credit,
                    OrgBranchName = apPettyCashDistribution.OrgBranchName,
                    DateCreated = apPettyCashDistribution.DateCreated,
                    LastModified = apPettyCashDistribution.LastModified,
                    CreatedBy = apPettyCashDistribution.CreatedBy,
                    ModifiedBy = apPettyCashDistribution.ModifiedBy
                };

                _db.ApPettyCashDistributions.Attach(entity);
                _db.ApPettyCashDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apPettyCashDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
