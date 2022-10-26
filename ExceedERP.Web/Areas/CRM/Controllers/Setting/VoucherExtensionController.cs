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

    [AuthorizeRoles(CRMRoles.CRMAdmin,CRMRoles.CashSalesUser, CRMRoles.CashSalesApprover , CRMRoles.SalesOrderUser, CRMRoles.SalesOrderApprover)] 
    public class VoucherExtensionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VoucherExtensions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VoucherExtension> voucherextensions = db.VoucherExtensions;
            DataSourceResult result = voucherextensions.ToDataSourceResult(request, voucherExtension => new {
                VoucherExtensionID = voucherExtension.VoucherExtensionID,
                VoucherDefinition = voucherExtension.VoucherDefinition,
                Description = voucherExtension.Description,
                Index = voucherExtension.Index,
                IsMandatory = voucherExtension.IsMandatory,
                CreatedBy = voucherExtension.CreatedBy,
                DateCreated = voucherExtension.DateCreated,
                LastModified = voucherExtension.LastModified,
                ModifiedBy = voucherExtension.ModifiedBy,
                OrgBranchName = voucherExtension.OrgBranchName,
                Remark = voucherExtension.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensions_Create([DataSourceRequest]DataSourceRequest request, VoucherExtension voucherExtension)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtension
                {
                    VoucherDefinition = voucherExtension.VoucherDefinition,
                    Description = voucherExtension.Description,
                    Index = voucherExtension.Index,
                    IsMandatory = voucherExtension.IsMandatory,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = voucherExtension.Remark
                };

                db.VoucherExtensions.Add(entity);
                db.SaveChanges();
                voucherExtension.VoucherExtensionID = entity.VoucherExtensionID;
            }

            return Json(new[] { voucherExtension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensions_Update([DataSourceRequest]DataSourceRequest request, VoucherExtension voucherExtension)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtension
                {
                    VoucherExtensionID = voucherExtension.VoucherExtensionID,
                    VoucherDefinition = voucherExtension.VoucherDefinition,
                    Description = voucherExtension.Description,
                    Index = voucherExtension.Index,
                    IsMandatory = voucherExtension.IsMandatory,
                    Remark = voucherExtension.Remark
                };

                db.VoucherExtensions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { voucherExtension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherExtensions_Destroy([DataSourceRequest]DataSourceRequest request, VoucherExtension voucherExtension)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherExtension
                {
                    VoucherExtensionID = voucherExtension.VoucherExtensionID,
                    VoucherDefinition = voucherExtension.VoucherDefinition,
                    Description = voucherExtension.Description,
                    Index = voucherExtension.Index,
                    IsMandatory = voucherExtension.IsMandatory,
                    Remark = voucherExtension.Remark
                };

                db.VoucherExtensions.Attach(entity);
                db.VoucherExtensions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Voucher Extension number " + voucherExtension.VoucherExtensionID);
      
            }

            return Json(new[] { voucherExtension }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
