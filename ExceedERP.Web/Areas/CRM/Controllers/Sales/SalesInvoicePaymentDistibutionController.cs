using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
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
    [AuthorizeRoles(CRMRoles.CashSalesApprover, CRMRoles.CashSalesUser)]
    public class SalesInvoicePaymentDistibutionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Distrubtions_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == Id).OrderByDescending(d => d.Debit).ToList();
            if (distributions.Any())
            {
                return Json(distributions.Select(s => new SalesInvoicePaymentDistibution
                {
                    SalesInvoicePaymentDistibutionId = s.SalesInvoicePaymentDistibutionId,
                    CashSalesInvoiceId = s.CashSalesInvoiceId,
                    AccountCode = s.AccountCode,
                    AccountDesc = s.AccountDesc,
                    Credit = s.Credit,
                    Debit = s.Debit,
                    AdditionalDescription = s.AdditionalDescription,

                    DateCreated = s.DateCreated,
                    CreatedBy = s.CreatedBy,
                    LastModified = s.LastModified,
                    ModifiedBy = s.ModifiedBy
                }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, SalesInvoicePaymentDistibution distribution, int Id)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var headerInvoice = db.CashSalesInvoices.Find(Id);
            string branchSeg = HrHelper.GetBranchLocationAccount(headerInvoice != null ? headerInvoice.BranchId : 0);
            string newacc = isReplacingBranchDisabled? distribution.AccountCode : _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);


            if (ModelState.IsValid)
            {
                var entity = new SalesInvoicePaymentDistibution
                {
                    SalesInvoicePaymentDistibutionId = distribution.SalesInvoicePaymentDistibutionId,
                    CashSalesInvoiceId = Id,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,
                    AdditionalDescription = distribution.AdditionalDescription,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.SalesInvoicePaymentDistibutions.Add(entity);
                db.SaveChanges();

                distribution.SalesInvoicePaymentDistibutionId = entity.SalesInvoicePaymentDistibutionId;
            }
            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, SalesInvoicePaymentDistibution distribution)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var headerInvoice = db.CashSalesInvoices.Find(distribution.CashSalesInvoiceId);
            string branchSeg = HrHelper.GetBranchLocationAccount(headerInvoice != null ? headerInvoice.BranchId : 0);
            string newacc = isReplacingBranchDisabled ? distribution.AccountCode : _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);

            if (ModelState.IsValid)
            {
                var entity = new SalesInvoicePaymentDistibution
                {
                    SalesInvoicePaymentDistibutionId = distribution.SalesInvoicePaymentDistibutionId,
                    CashSalesInvoiceId = distribution.CashSalesInvoiceId,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,
                    AdditionalDescription = distribution.AdditionalDescription,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesInvoicePaymentDistibutions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, SalesInvoicePaymentDistibution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesInvoicePaymentDistibution
                {
                    SalesInvoicePaymentDistibutionId = distribution.SalesInvoicePaymentDistibutionId,
                    CashSalesInvoiceId = distribution.CashSalesInvoiceId,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,
                    AdditionalDescription = distribution.AdditionalDescription,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = distribution.LastModified,
                    ModifiedBy = distribution.ModifiedBy
                };

                db.SalesInvoicePaymentDistibutions.Attach(entity);
                db.SalesInvoicePaymentDistibutions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}