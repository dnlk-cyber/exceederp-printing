using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesOrderApprover, CRMRoles.SalesOrderUser)]
    public class PaymentScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucher, VoucherType type)
        {
            IQueryable<PaymentSchedule> lineItems = db.PaymentSchedule.Where(c => c.VoucherId == voucher && c.VoucherType == type);
            DataSourceResult result = lineItems.ToDataSourceResult(request, paymentSchedule => new PaymentSchedule
            {
                PaymentScheduleId = paymentSchedule.PaymentScheduleId,
                Date = paymentSchedule.Date,
                Remark = paymentSchedule.Remark,
                VoucherId = paymentSchedule.VoucherId,
                VoucherType = paymentSchedule.VoucherType,
                Description = paymentSchedule.Description,
                Amount =  paymentSchedule.Amount,

                CreatedBy = paymentSchedule.CreatedBy,
                DateCreated = paymentSchedule.DateCreated,
                LastModified = paymentSchedule.LastModified,
                ModifiedBy = paymentSchedule.ModifiedBy,
                OrgBranchName = paymentSchedule.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Create([DataSourceRequest]DataSourceRequest request, PaymentSchedule paymentSchedule, int voucher, VoucherType type)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentSchedule
                {
                    PaymentScheduleId = paymentSchedule.PaymentScheduleId,
                    Date = paymentSchedule.Date,
                    Remark = paymentSchedule.Remark,
                    VoucherId = voucher,
                    VoucherType = type,
                    Description = paymentSchedule.Description,
                    Amount = paymentSchedule.Amount,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };

                db.PaymentSchedule.Add(entity);
                db.SaveChanges();
                paymentSchedule.PaymentScheduleId = entity.PaymentScheduleId;
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Create Payment Schedule Line Item with id " + paymentSchedule.PaymentScheduleId, Modules.CRM);

            }

            return Json(new[] { paymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Update([DataSourceRequest]DataSourceRequest request, PaymentSchedule paymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentSchedule
                {
                    PaymentScheduleId = paymentSchedule.PaymentScheduleId,
                    Date = paymentSchedule.Date,
                    Remark = paymentSchedule.Remark,
                    VoucherId = paymentSchedule.VoucherId,
                    VoucherType = paymentSchedule.VoucherType,
                    Description = paymentSchedule.Description,
                    Amount = paymentSchedule.Amount,

                    CreatedBy = paymentSchedule.CreatedBy,
                    DateCreated = paymentSchedule.DateCreated,
                    LastModified = paymentSchedule.LastModified,
                    ModifiedBy = paymentSchedule.ModifiedBy,
                    OrgBranchName = paymentSchedule.OrgBranchName
                };

                db.PaymentSchedule.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated Payment Schedule Line Item with id " + paymentSchedule.PaymentScheduleId, Modules.CRM);

            }

            return Json(new[] { paymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Destroy([DataSourceRequest]DataSourceRequest request, PaymentSchedule paymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentSchedule
                {
                    PaymentScheduleId = paymentSchedule.PaymentScheduleId,
                    Date = paymentSchedule.Date,
                    Remark = paymentSchedule.Remark,
                    VoucherId = paymentSchedule.VoucherId,
                    VoucherType = paymentSchedule.VoucherType,
                    Description = paymentSchedule.Description,
                    Amount = paymentSchedule.Amount,

                    CreatedBy = paymentSchedule.CreatedBy,
                    DateCreated = paymentSchedule.DateCreated,
                    LastModified = paymentSchedule.LastModified,
                    ModifiedBy = paymentSchedule.ModifiedBy,
                    OrgBranchName = paymentSchedule.OrgBranchName

                };

                db.PaymentSchedule.Attach(entity);
                db.PaymentSchedule.Remove(entity);
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment Schedule Line Item with id " + paymentSchedule.PaymentScheduleId, Modules.CRM);

            }

            return Json(new[] { paymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}