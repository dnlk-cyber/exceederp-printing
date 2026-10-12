using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser)]
    public class LeaveEntitlementSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/LeaveEntitlementSetting
       
        #region Read
        public ActionResult LeaveEntitlementSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<LeaveEntitlementSetting> query = db.LeaveEntitlementSettings;

            var result = query.ToDataSourceResult(request, x => new
            {
                x.LeaveEntitlementSettingId,
                x.GlFiscalYearId,
                x.IsGenerated,
                x.IsRegeneratingAllowed,
                x.LeaveIncrementAmount,
                x.Remark,
                x.CreatedBy,
                x.DateCreated,
                x.ModifiedBy,
                x.LastModified
            });

            return Json(result);
        }
        #endregion

        #region Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitlementSettings_Create([DataSourceRequest] DataSourceRequest request, LeaveEntitlementSetting model)
        {
            // check duplication (by fiscal year)
            if (db.LeaveEntitlementSettings.Any(x => x.GlFiscalYearId == model.GlFiscalYearId))
            {
                ModelState.AddModelError("exist", "Record already exists for this fiscal year.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = Map(model);

                db.LeaveEntitlementSettings.Add(entity);
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitlement Setting",
                    "Success",
                    User.Identity.Name,
                    "Leave Entitlement Setting Created by " + User.Identity.Name,
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Update
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitlementSettings_Update([DataSourceRequest] DataSourceRequest request, LeaveEntitlementSetting model)
        {
            if (db.LeaveEntitlementSettings.Any(x =>
                 x.GlFiscalYearId == model.GlFiscalYearId &&
                 x.LeaveEntitlementSettingId != model.LeaveEntitlementSettingId))
            {
                ModelState.AddModelError("exist", "Duplicate record for this fiscal year is not allowed.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = Map(model);

                db.LeaveEntitlementSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitlement Setting",
                    "Success",
                    User.Identity.Name,
                    "Leave Entitlement Setting Updated by " + User.Identity.Name,
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitlementSettings_Destroy([DataSourceRequest] DataSourceRequest request, LeaveEntitlementSetting model)
        {
            if (ModelState.IsValid)
            {
                var entity = Map(model);

                db.LeaveEntitlementSettings.Attach(entity);
                db.LeaveEntitlementSettings.Remove(entity);
                db.SaveChanges();

                // delete log (optional)
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete",
                    "Success", User.Identity.Name,
                   "Leave Entitlement Setting Deleted");
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Mapping Method
        private LeaveEntitlementSetting Map(LeaveEntitlementSetting x)
        {
            return new LeaveEntitlementSetting
            {
                LeaveEntitlementSettingId = x.LeaveEntitlementSettingId,
                GlFiscalYearId = x.GlFiscalYearId,
                IsGenerated = x.IsGenerated,
                IsRegeneratingAllowed = x.IsRegeneratingAllowed,
                LeaveIncrementAmount = x.LeaveIncrementAmount,
                Remark = x.Remark,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                ModifiedBy = x.ModifiedBy,
                LastModified = x.LastModified
            };
        }
        #endregion

        #region Export
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export",
                "Success", User.Identity.Name, "Exported Leave Entitlement Settings");

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export",
                "Success", User.Identity.Name, "Exported Leave Entitlement Settings");

            return File(fileContents, contentType, fileName);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}