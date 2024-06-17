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
    public class ProjectKPIsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectKPIs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectKPI> projectkpis = db.ProjectKPIs;
            DataSourceResult result = projectkpis.ToDataSourceResult(request, projectKPI => new {
                ProjectKPIId = projectKPI.ProjectKPIId,
                KPIName = projectKPI.KPIName,
                ProjectKPIType = projectKPI.ProjectKPIType,
                Description = projectKPI.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectKPIs_Create([DataSourceRequest]DataSourceRequest request, ProjectKPI projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectKPI);
                db.ProjectKPIs.Add(entity);
                db.SaveChanges();
                projectKPI.ProjectKPIId = entity.ProjectKPIId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Project Actual", "Success", User.Identity.Name, entity.KPIName + " is Created By  " + User.Identity.Name, Modules.Planning);
                #endregion
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectKPIs_Update([DataSourceRequest]DataSourceRequest request, ProjectKPI projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectKPI);
                db.ProjectKPIs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectKPIs_Destroy([DataSourceRequest]DataSourceRequest request, ProjectKPI projectKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectKPI);
                db.ProjectKPIs.Attach(entity);
                db.ProjectKPIs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectKPI }.ToDataSourceResult(request, ModelState));
        }
        private ProjectKPI GetMappedObject(ProjectKPI projectKPI)
        {
            var entity = new ProjectKPI
            {
                ProjectKPIId = projectKPI.ProjectKPIId,
                KPIName = projectKPI.KPIName,
                ProjectKPIType = projectKPI.ProjectKPIType,
                Description = projectKPI.Description
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
