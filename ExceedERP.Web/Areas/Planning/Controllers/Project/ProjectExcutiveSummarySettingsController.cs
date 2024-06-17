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

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectExcutiveSummarySettingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectExcutiveSummarySettings_Read([DataSourceRequest]DataSourceRequest request,int kId)
        {
            IQueryable<ProjectExcutiveSummarySetting> projectsubkpis = db.ProjectExcutiveSummarySettings.Where(x=>x.ProjectExcutiveSettingId ==kId);
            DataSourceResult result = projectsubkpis.ToDataSourceResult(request, projectSubKPI => new {
                ProjectExcutiveSummarySettingId = projectSubKPI.ProjectExcutiveSummarySettingId,
                ProjectExcutiveSettingId = projectSubKPI.ProjectExcutiveSettingId,
                ProjectKPIId = projectSubKPI.ProjectKPIId,
                ProjectSubKPIId = projectSubKPI.ProjectSubKPIId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSummarySettings_Create([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSummarySetting projectSubKPI,int kId)
        {
            if (ModelState.IsValid)
            {
                projectSubKPI.ProjectExcutiveSettingId = kId;
                var entity = GetMappedObject(projectSubKPI);
                db.ProjectExcutiveSummarySettings.Add(entity);
                db.SaveChanges();
                projectSubKPI.ProjectSubKPIId = entity.ProjectSubKPIId;
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSummarySettings_Update([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSummarySetting projectSubKPI)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(projectSubKPI);
                db.ProjectExcutiveSummarySettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectExcutiveSummarySettings_Destroy([DataSourceRequest]DataSourceRequest request, ProjectExcutiveSummarySetting projectSubKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectSubKPI);
                db.ProjectExcutiveSummarySettings.Attach(entity);
                db.ProjectExcutiveSummarySettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }
        private ProjectExcutiveSummarySetting GetMappedObject(ProjectExcutiveSummarySetting projectSubKPI)
        {
            var entity = new ProjectExcutiveSummarySetting
            {
                ProjectExcutiveSummarySettingId = projectSubKPI.ProjectExcutiveSummarySettingId,
                ProjectExcutiveSettingId = projectSubKPI.ProjectExcutiveSettingId,
                ProjectKPIId = projectSubKPI.ProjectKPIId,
                ProjectSubKPIId = projectSubKPI.ProjectSubKPIId
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
