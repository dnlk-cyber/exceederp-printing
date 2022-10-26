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
    public class VoucherValueController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VoucherValues_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<VoucherValue> vouchervalues = db.VoucherValues.Where(vv => vv.Voucher == voucherID.ToString());
            DataSourceResult result = vouchervalues.ToDataSourceResult(request, voucherValue => new {
                VoucherValueID = voucherValue.VoucherValueID,
                Voucher = voucherValue.Voucher,
                Discount = voucherValue.Discount,
                SubTotal = voucherValue.SubTotal,
                AdditionalCharge = voucherValue.AdditionalCharge,
                CreatedBy = voucherValue.CreatedBy,
                DateCreated = voucherValue.DateCreated,
                LastModified = voucherValue.LastModified,
                ModifiedBy = voucherValue.ModifiedBy,
                OrgBranchName = voucherValue.OrgBranchName,
                Remark = voucherValue.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherValues_Create([DataSourceRequest]DataSourceRequest request, VoucherValue voucherValue, int voucherID)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherValue
                {
                   VoucherValueID = voucherValue.VoucherValueID,
                   Voucher = voucherValue.Voucher,
                    Discount = voucherValue.Discount,
                    SubTotal = voucherValue.SubTotal,
                    AdditionalCharge = voucherValue.AdditionalCharge,
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                    Remark = voucherValue.Remark
                };

                db.VoucherValues.Add(entity);
                db.SaveChanges();
                voucherValue.VoucherValueID = entity.VoucherValueID;
            }

            return Json(new[] { voucherValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherValues_Update([DataSourceRequest]DataSourceRequest request, VoucherValue voucherValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherValue
                {
                    VoucherValueID = voucherValue.VoucherValueID,
                    Voucher = voucherValue.Voucher,
                    Discount = voucherValue.Discount,
                    SubTotal = voucherValue.SubTotal,
                    AdditionalCharge = voucherValue.AdditionalCharge,
                    Remark = voucherValue.Remark
                };

                db.VoucherValues.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { voucherValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherValues_Destroy([DataSourceRequest]DataSourceRequest request, VoucherValue voucherValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherValue
                {
                    VoucherValueID = voucherValue.VoucherValueID,
                    Voucher = voucherValue.Voucher,
                    Discount = voucherValue.Discount,
                    SubTotal = voucherValue.SubTotal,
                    AdditionalCharge = voucherValue.AdditionalCharge,
                    Remark = voucherValue.Remark
                };

                db.VoucherValues.Attach(entity);
                db.VoucherValues.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Voucher Value number " + voucherValue.VoucherValueID);
            }

            return Json(new[] { voucherValue }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
