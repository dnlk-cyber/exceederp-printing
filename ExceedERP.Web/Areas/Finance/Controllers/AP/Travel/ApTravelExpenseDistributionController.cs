using System;
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
        private AccountHelper helper = new AccountHelper();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelExpenseDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseDistribution> ApTravelExpenseDistributions = _db.ApTravelExpenseDistributions.Where(x=>x.ApTravelExpenseClaimId==id);
            DataSourceResult result = ApTravelExpenseDistributions.ToDataSourceResult(request, ApTravelExpenseDistribution => new {
                ApTravelExpenseDistributionId = ApTravelExpenseDistribution.ApTravelExpenseDistributionId,
                ApTravelExpenseClaimId = ApTravelExpenseDistribution.ApTravelExpenseClaimId,
                AccountCode = ApTravelExpenseDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(ApTravelExpenseDistribution.AccountCode),
                AdditionalDescription = ApTravelExpenseDistribution.AdditionalDescription,
                Debit = ApTravelExpenseDistribution.Debit,
                Credit = ApTravelExpenseDistribution.Credit,
                OrgBranchName = ApTravelExpenseDistribution.OrgBranchName,
                DateCreated = ApTravelExpenseDistribution.DateCreated,
                LastModified = ApTravelExpenseDistribution.LastModified,
                CreatedBy = ApTravelExpenseDistribution.CreatedBy,
                ModifiedBy = ApTravelExpenseDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution ApTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseClaimId =id,
                    AccountCode = ApTravelExpenseDistribution.AccountCode,
                    AccountDesc = helper.GetAccountDesc(ApTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = ApTravelExpenseDistribution.AdditionalDescription,
                    Debit = ApTravelExpenseDistribution.Debit,
                    Credit = ApTravelExpenseDistribution.Credit,
                    OrgBranchName = ApTravelExpenseDistribution.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = ApTravelExpenseDistribution.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = ApTravelExpenseDistribution.ModifiedBy
                };

                _db.ApTravelExpenseDistributions.Add(entity);
                _db.SaveChanges();
                ApTravelExpenseDistribution.ApTravelExpenseDistributionId = entity.ApTravelExpenseDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash distribution", "Success", User.Identity.Name, $"created petty cash distribution {entity.AccountCode} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { ApTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution ApTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseClaimId = ApTravelExpenseDistribution.ApTravelExpenseClaimId,
                    ApTravelExpenseDistributionId = ApTravelExpenseDistribution.ApTravelExpenseDistributionId,
                    AccountCode = ApTravelExpenseDistribution.AccountCode,
                    AccountDesc = helper.GetAccountDesc(ApTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = ApTravelExpenseDistribution.AdditionalDescription,
                    Debit = ApTravelExpenseDistribution.Debit,
                    Credit = ApTravelExpenseDistribution.Credit,
                    OrgBranchName = ApTravelExpenseDistribution.OrgBranchName,
                    DateCreated = ApTravelExpenseDistribution.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = ApTravelExpenseDistribution.CreatedBy,
                    ModifiedBy =User.Identity.Name
                };

                _db.ApTravelExpenseDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash distribution", "Success", User.Identity.Name, $"updated petty cash distribution {entity.AccountCode} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { ApTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseDistribution ApTravelExpenseDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseDistribution
                {
                    ApTravelExpenseClaimId = ApTravelExpenseDistribution.ApTravelExpenseClaimId,
                    ApTravelExpenseDistributionId = ApTravelExpenseDistribution.ApTravelExpenseDistributionId,
                    AccountCode = ApTravelExpenseDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(ApTravelExpenseDistribution.AccountCode),
                    AdditionalDescription = ApTravelExpenseDistribution.AdditionalDescription,
                    Debit = ApTravelExpenseDistribution.Debit,
                    Credit = ApTravelExpenseDistribution.Credit,
                    OrgBranchName = ApTravelExpenseDistribution.OrgBranchName,
                    DateCreated = ApTravelExpenseDistribution.DateCreated,
                    LastModified = ApTravelExpenseDistribution.LastModified,
                    CreatedBy = ApTravelExpenseDistribution.CreatedBy,
                    ModifiedBy = ApTravelExpenseDistribution.ModifiedBy
                };

                _db.ApTravelExpenseDistributions.Attach(entity);
                _db.ApTravelExpenseDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { ApTravelExpenseDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
