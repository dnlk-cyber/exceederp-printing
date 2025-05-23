using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.AdvancePaymentUser, CRMRoles.AdvancePaymentApprover, FinanceConstants.GeneralSubLedgerTransfer)]
    public class AdvancePaymentLineController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AdvancePaymentLines_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var AdvancePaymentLines = db.AdvancePaymentLines.Where(x => x.AdvancePaymentId == Id).Select(s => new
            {
                AdvancePaymentLineId = s.AdvancePaymentLineId,
                AdvancePaymentId = s.AdvancePaymentId,
                RecieveMethod = s.RecieveMethod,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                Value = s.Value,
                PaymentMethod = s.PaymentMethod
            });
            DataSourceResult result = AdvancePaymentLines.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Create([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine, int Id)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            if (ModelState.IsValid)
            {
                var entity = new AdvancePaymentLine
                {
                    RecieveMethod = AdvancePaymentLine.RecieveMethod,
                    Value = AdvancePaymentLine.Value,
                    CreatedBy = User.Identity.Name,
                    AdvancePaymentId = Id,
                    DateCreated = DateTime.Now
                };

                db.AdvancePaymentLines.Add(entity);
                db.SaveChanges();

                var header = this.GetAdvancePaymentHeader(entity.AdvancePaymentId);
                if (header != null)
                {
                    UpdateHeaderOnLineChange(header);
                    new CRMPaymentDistibutionHelper().UpdateAdvancePaymentDistribution(header);
                }

            }
            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Update([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvancePaymentLine
                {
                    AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
                    AdvancePaymentId = AdvancePaymentLine.AdvancePaymentId,
                    RecieveMethod = AdvancePaymentLine.RecieveMethod,
                    Value = AdvancePaymentLine.Value,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                db.AdvancePaymentLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var header = this.GetAdvancePaymentHeader(entity.AdvancePaymentId);
                if (header != null)
                {
                    UpdateHeaderOnLineChange(header);

                    new CRMPaymentDistibutionHelper().UpdateAdvancePaymentDistribution(header);
                }
            }


            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Destroy([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvancePaymentLine
                {
                    AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
                    Value = AdvancePaymentLine.Value
                };
                db.AdvancePaymentLines.Attach(entity);
                db.AdvancePaymentLines.Remove(entity);
                db.SaveChanges();

                var header = this.GetAdvancePaymentHeader(entity.AdvancePaymentId);
                if (header != null)
                {
                    UpdateHeaderOnLineChange(header);
                    new CRMPaymentDistibutionHelper().UpdateAdvancePaymentDistribution(header);
                }

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   Advance Payment number " + AdvancePaymentLine.AdvancePaymentLineId);

            }

            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        private AdvancePayment GetAdvancePaymentHeader(int id)
        {
            return db.AdvancePayments.Find(id);
        }
        private void UpdateHeaderOnLineChange(AdvancePayment header)
        {
            if(header != null)
            {
                var lines = db.AdvancePaymentLines
                    .AsNoTracking()
                    .Where(l => l.AdvancePaymentId == header.AdvancePaymentId)
                    .ToList();
                header.GrandTotal = lines.Sum(l => l.Value);
                header.Outstanding = header.GrandTotal - header.UsedAmount;

                db.AdvancePayments.Attach(header);
                db.Entry(header).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}