using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesApprover, CRMRoles.CashSalesUser, FinanceConstants.GeneralSubLedgerTransfer)]
    public class CRMDepositDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Distributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CRMDepositDistribution> revenueDistributions = db.CRMDepositDistribution.Where(x =>  x.CRMDepositId == id);

            DataSourceResult result = revenueDistributions.ToDataSourceResult(request, s => new CRMDepositDistribution
            {
                CRMDepositDistributionId = s.CRMDepositDistributionId,
                CRMDepositId = s.CRMDepositId,
                AccountCode = s.AccountCode,
                AccountDesc = s.AccountDesc,
                AdditionalDescription = s.AdditionalDescription,
                Credit = s.Credit,
                Debit = s.Debit,
            });
            return Json(result);
            
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, CRMDepositDistribution distribution, int id)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var depositHeader = db.CRMDeposit.Find(id);
            string branchSeg = HrHelper.GetBranchLocationAccount(depositHeader != null ? depositHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);

            if (ModelState.IsValid)
            {
                var entity = new CRMDepositDistribution
                {
                    CRMDepositDistributionId = distribution.CRMDepositId,
                    CRMDepositId = id,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.CRMDepositDistribution.Add(entity);
                db.SaveChanges();

                distribution.CRMDepositDistributionId = entity.CRMDepositDistributionId;
            }
            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, CRMDepositDistribution distribution)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var depositHeader = db.CRMDeposit.Find(distribution.CRMDepositId);
            string branchSeg = HrHelper.GetBranchLocationAccount(depositHeader != null ? depositHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);

            if (ModelState.IsValid)
            {
                var entity = new CRMDepositDistribution
                {
                    CRMDepositDistributionId = distribution.CRMDepositDistributionId,
                    CRMDepositId = distribution.CRMDepositId,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.CRMDepositDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, CRMDepositDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new CRMDepositDistribution
                {
                    CRMDepositDistributionId = distribution.CRMDepositDistributionId,
                    CRMDepositId = distribution.CRMDepositId,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = distribution.LastModified,
                    ModifiedBy = distribution.ModifiedBy
                };

                db.CRMDepositDistribution.Attach(entity);
                db.CRMDepositDistribution.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

    }
}