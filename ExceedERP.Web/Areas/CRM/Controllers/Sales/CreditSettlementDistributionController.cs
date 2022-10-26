using ExceedERP.Core.Domain.CRM;
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
    [AuthorizeRoles(CRMRoles.CreditSettlementUser)]
    public class CreditSettlementDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: CRM/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreditSettlementDistribution_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var advancePayment = db.CreditSettlementDistribution.Where(x => x.CreditSettlementId == Id).ToList();


            return Json(advancePayment.Select(s => new CreditSettlementDistribution
            {
                CreditSettlementDistributionId = s.CreditSettlementDistributionId,
                CreditSettlementId = s.CreditSettlementId,
                AccountCode = s.AccountCode,
                AccountDesc = s.AccountDesc,
                AdditionalDescription = s.AdditionalDescription,
                Credit = s.Credit,
                Debit = s.Debit,
            }
            ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, CreditSettlementDistribution distribution, int id)
        {

            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var settlmentHeader = db.CreditSettlement.Find(distribution.CreditSettlementId);
            string branchSeg = HrHelper.GetBranchLocationAccount(settlmentHeader != null ? settlmentHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);


            if (ModelState.IsValid)
            {
                var entity = new CreditSettlementDistribution
                {
                    CreditSettlementDistributionId = distribution.CreditSettlementDistributionId,
                    CreditSettlementId = id,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.CreditSettlementDistribution.Add(entity);
                db.SaveChanges();

                distribution.CreditSettlementDistributionId = entity.CreditSettlementDistributionId;
            }
            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, CreditSettlementDistribution distribution)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var settlmentHeader = db.CreditSettlement.Find(distribution.CreditSettlementId);
            string branchSeg = HrHelper.GetBranchLocationAccount(settlmentHeader != null ? settlmentHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);


            if (ModelState.IsValid)
            {
                var entity = new CreditSettlementDistribution
                {
                    CreditSettlementDistributionId = distribution.CreditSettlementDistributionId,
                    CreditSettlementId = distribution.CreditSettlementId,
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

                db.CreditSettlementDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, CreditSettlementDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new CreditSettlementDistribution
                {
                    CreditSettlementDistributionId = distribution.CreditSettlementDistributionId,
                    CreditSettlementId = distribution.CreditSettlementId,
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

                db.CreditSettlementDistribution.Attach(entity);
                db.CreditSettlementDistribution.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

    }
}