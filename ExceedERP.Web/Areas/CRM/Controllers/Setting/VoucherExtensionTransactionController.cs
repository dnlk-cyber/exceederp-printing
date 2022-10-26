using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class VoucherExtensionTransactionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VoucherExtensionTransactions_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<VoucherExtensionTransaction> voucherextensiontransactions = db.VoucherExtensionTransactions.Where(vt => vt.Voucher == voucherID.ToString());
            DataSourceResult result = voucherextensiontransactions.ToDataSourceResult(request, voucherExtensionTransaction => new {
                VoucherExtensionTransactionID = voucherExtensionTransaction.VoucherExtensionTransactionID,
                Voucher = voucherExtensionTransaction.Voucher,
                VoucherExtension = voucherExtensionTransaction.VoucherExtension,
                Number = voucherExtensionTransaction.Number,
                CreatedBy = voucherExtensionTransaction.CreatedBy,
                DateCreated = voucherExtensionTransaction.DateCreated,
                LastModified = voucherExtensionTransaction.LastModified,
                ModifiedBy = voucherExtensionTransaction.ModifiedBy,
                OrgBranchName = voucherExtensionTransaction.OrgBranchName,
                Remark = voucherExtensionTransaction.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensionTransactions_Create([DataSourceRequest]DataSourceRequest request, VoucherExtensionTransaction voucherExtensionTransaction, int voucherID)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtensionTransaction
                {
                    Voucher = voucherID.ToString(),
                    VoucherExtension = voucherExtensionTransaction.VoucherExtension,
                    Number = voucherExtensionTransaction.Number,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = voucherExtensionTransaction.Remark
                };

                db.VoucherExtensionTransactions.Add(entity);
                db.SaveChanges();
                voucherExtensionTransaction.VoucherExtensionTransactionID = entity.VoucherExtensionTransactionID;
            }

            return Json(new[] { voucherExtensionTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensionTransactions_Update([DataSourceRequest]DataSourceRequest request, VoucherExtensionTransaction voucherExtensionTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtensionTransaction
                {
                    VoucherExtensionTransactionID = voucherExtensionTransaction.VoucherExtensionTransactionID,
                    Voucher = voucherExtensionTransaction.Voucher,
                    VoucherExtension = voucherExtensionTransaction.VoucherExtension,
                    Number = voucherExtensionTransaction.Number,
                    Remark = voucherExtensionTransaction.Remark
                };

                db.VoucherExtensionTransactions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { voucherExtensionTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensionTransactions_Destroy([DataSourceRequest]DataSourceRequest request, VoucherExtensionTransaction voucherExtensionTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtensionTransaction
                {
                    VoucherExtensionTransactionID = voucherExtensionTransaction.VoucherExtensionTransactionID,
                    Voucher = voucherExtensionTransaction.Voucher,
                    VoucherExtension = voucherExtensionTransaction.VoucherExtension,
                    Number = voucherExtensionTransaction.Number,
                    Remark = voucherExtensionTransaction.Remark
                };

                db.VoucherExtensionTransactions.Attach(entity);
                db.VoucherExtensionTransactions.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Voucher Extension Transaction number " + voucherExtensionTransaction.VoucherExtensionTransactionID);
      
            }

            return Json(new[] { voucherExtensionTransaction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
