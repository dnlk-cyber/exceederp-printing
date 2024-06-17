﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning.Project;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectExcutiveSettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["projectKpIs"] = db.ProjectKPIs.Select(x => new { x.ProjectKPIId, Name = x.KPIName }).ToList();
            ViewData["projectSubKpIs"] = db.ProjectSubKPIs.Select(x => new { x.ProjectSubKPIId, Name = x.SubKPIName }).ToList();
            return View();
        }

        public ActionResult ProjectExcutiveSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectExcutiveSetting> projectkpis = db.ProjectExcutiveSettings;
            DataSourceResult result = projectkpis.ToDataSourceResult(request, projectKPI => new {
                ProjectExcutiveSettingId = projectKPI.ProjectExcutiveSettingId,
                Category = projectKPI.Category,
                Description = projectKPI.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSettings_Create([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSetting projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectExcutiveSetting
                {
                    ProjectExcutiveSettingId = projectKPI.ProjectExcutiveSettingId,
                    Category = projectKPI.Category,
                    Description = projectKPI.Description
                };

                db.ProjectExcutiveSettings.Add(entity);
                db.SaveChanges();
                projectKPI.ProjectExcutiveSettingId = entity.ProjectExcutiveSettingId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, entity.ProjectExcutiveSettingId + " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSettings_Update([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSetting projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectExcutiveSetting
                {
                    ProjectExcutiveSettingId = projectKPI.ProjectExcutiveSettingId,
                    Category = projectKPI.Category,
                    Description = projectKPI.Description
                };

                db.ProjectExcutiveSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSettings_Destroy([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSetting projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectExcutiveSetting
                {
                    ProjectExcutiveSettingId = projectKPI.ProjectExcutiveSettingId,
                    Category = projectKPI.Category,
                    Description = projectKPI.Description
                };

                db.ProjectExcutiveSettings.Attach(entity);
                db.ProjectExcutiveSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
