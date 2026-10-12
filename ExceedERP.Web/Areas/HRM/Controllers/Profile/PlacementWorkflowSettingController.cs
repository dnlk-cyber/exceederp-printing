using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin,HrmConstants.HRAdmin)]
    public class PlacementWorkflowSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // READ
        public ActionResult PlacementWorkflowSettings_Read(
            [DataSourceRequest] DataSourceRequest request)
        {
            var settings = db.PlacementWorkflowSettings
                .AsQueryable();

            DataSourceResult result =
                settings.ToDataSourceResult(
                    request,
                    setting => new
                    {
                        PlacementWorkflowSettingId =
                            setting.PlacementWorkflowSettingId,

                        RequireChecker =
                            setting.RequireChecker,

                        DateCreated =
                            setting.DateCreated,

                        LastModified =
                            setting.LastModified,

                        CreatedBy =
                            setting.CreatedBy,

                        ModifiedBy =
                            setting.ModifiedBy
                    });

            return Json(result);
        }

        // CREATE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementWorkflowSettings_Create(
            [DataSourceRequest] DataSourceRequest request,
            PlacementWorkflowSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                // Only one global setting should exist
                var existingSetting =
                    db.PlacementWorkflowSettings.FirstOrDefault();

                if (existingSetting != null)
                {
                    ModelState.AddModelError(
                        "",
                        "Placement workflow setting already exists. Please update the existing setting.");
                }
                else
                {
                    var entity = new PlacementWorkflowSetting
                    {
                        RequireChecker = setting.RequireChecker,

                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,

                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name
                    };

                    db.PlacementWorkflowSettings.Add(entity);
                    db.SaveChanges();

                    setting.PlacementWorkflowSettingId =
                        entity.PlacementWorkflowSettingId;

                    setting.DateCreated =
                        entity.DateCreated;

                    setting.LastModified =
                        entity.LastModified;

                    setting.CreatedBy =
                        entity.CreatedBy;

                    setting.ModifiedBy =
                        entity.ModifiedBy;
                }
            }

            return Json(
                new[] { setting }
                    .ToDataSourceResult(request, ModelState));
        }

        // UPDATE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementWorkflowSettings_Update(
            [DataSourceRequest] DataSourceRequest request,
            PlacementWorkflowSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity =
                    db.PlacementWorkflowSettings
                        .FirstOrDefault(
                            x => x.PlacementWorkflowSettingId ==
                                 setting.PlacementWorkflowSettingId);

                if (entity == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Placement workflow setting was not found.");
                }
                else
                {
                    entity.RequireChecker =
                        setting.RequireChecker;

                    entity.LastModified =
                        DateTime.Now;

                    entity.ModifiedBy =
                        User.Identity.Name;

                    db.SaveChanges();
                }
            }

            return Json(
                new[] { setting }
                    .ToDataSourceResult(request, ModelState));
        }

        // DELETE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacementWorkflowSettings_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            PlacementWorkflowSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity =
                    db.PlacementWorkflowSettings
                        .FirstOrDefault(
                            x => x.PlacementWorkflowSettingId ==
                                 setting.PlacementWorkflowSettingId);

                if (entity == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Placement workflow setting was not found.");
                }
                else
                {
                    UserHelper.DeleteLog(
                        Request.UserHostAddress,
                        "Delete",
                        "Success",
                        User.Identity.Name,
                        "Deleted placement workflow setting information.");

                    db.PlacementWorkflowSettings.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(
                new[] { setting }
                    .ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}