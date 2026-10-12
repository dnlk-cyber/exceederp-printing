using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser,HrmConstants.HRAdmin)]
    public class AllowanceGenerationSettingsController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // ========================
        //       READ
        // ========================
        public ActionResult Settings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var query = db.AllowanceGenerationSettings;

            var result = query.ToDataSourceResult(request, x => new
            {
                x.AllowanceGenerationSettingId,
                x.AllowGenerationForNewEmployee,
                x.AllowRegenerationForUpdatedEmployee,
                x.Remark,
                x.CreatedBy,
                x.ModifiedBy,
                x.DateCreated,
                x.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // ========================
        //       CREATE
        // ========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Settings_Create([DataSourceRequest] DataSourceRequest request, AllowanceGenerationSetting model)
        {

            ModelState.Clear();
            var checkexistance = db.AllowanceGenerationSettings.FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This setting is already registered!!");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                var entity = new AllowanceGenerationSetting
                {
                    AllowGenerationForNewEmployee = model.AllowGenerationForNewEmployee,
                    AllowRegenerationForUpdatedEmployee = model.AllowRegenerationForUpdatedEmployee,
                    Remark = model.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.AllowanceGenerationSettings.Add(entity);
                db.SaveChanges();

                model.AllowanceGenerationSettingId = entity.AllowanceGenerationSettingId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ========================
        //       UPDATE
        // ========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Settings_Update([DataSourceRequest] DataSourceRequest request,
                                            AllowanceGenerationSetting model)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.AllowanceGenerationSettings
                               .FirstOrDefault(x => x.AllowanceGenerationSettingId == model.AllowanceGenerationSettingId);

                if (entity != null)
                {
                    entity.AllowGenerationForNewEmployee = model.AllowGenerationForNewEmployee;
                    entity.AllowRegenerationForUpdatedEmployee = model.AllowRegenerationForUpdatedEmployee;
                    entity.Remark = model.Remark;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ========================
        //       DELETE
        // ========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Settings_Destroy([DataSourceRequest] DataSourceRequest request,
                                             AllowanceGenerationSetting model)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.AllowanceGenerationSettings
                               .FirstOrDefault(x => x.AllowanceGenerationSettingId == model.AllowanceGenerationSettingId);

                if (entity != null)
                {
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success",
                                         User.Identity.Name,
                                         $"Deleted Allowance Generation Setting ID {model.AllowanceGenerationSettingId}");

                    db.AllowanceGenerationSettings.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
