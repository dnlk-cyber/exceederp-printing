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
    public class ProjectSubKPIController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectSubKPIs_Read([DataSourceRequest]DataSourceRequest request,int kId)
        {
            IQueryable<ProjectSubKPI> projectsubkpis = db.ProjectSubKPIs.Where(x=>x.ProjectKPIId ==kId);
            DataSourceResult result = projectsubkpis.ToDataSourceResult(request, projectSubKPI => new {
                ProjectSubKPIId = projectSubKPI.ProjectSubKPIId,
                SubKPIName = projectSubKPI.SubKPIName,
                Unit = projectSubKPI.Unit,
                KPIType = projectSubKPI.KPIType,
                Description = projectSubKPI.Description,
                ProjectKPIId = projectSubKPI.ProjectKPIId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectSubKPIs_Create([DataSourceRequest]DataSourceRequest request, ProjectSubKPI projectSubKPI,int kId)
        {
            if (ModelState.IsValid)
            {
                projectSubKPI.ProjectKPIId = kId;
                var entity = GetMappedObject(projectSubKPI);
                db.ProjectSubKPIs.Add(entity);
                db.SaveChanges();
                projectSubKPI.ProjectSubKPIId = entity.ProjectSubKPIId;
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectSubKPIs_Update([DataSourceRequest]DataSourceRequest request, ProjectSubKPI projectSubKPI)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(projectSubKPI);
                db.ProjectSubKPIs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectSubKPIs_Destroy([DataSourceRequest]DataSourceRequest request, ProjectSubKPI projectSubKPI)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(projectSubKPI);
                db.ProjectSubKPIs.Attach(entity);
                db.ProjectSubKPIs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectSubKPI }.ToDataSourceResult(request, ModelState));
        }
        private ProjectSubKPI GetMappedObject(ProjectSubKPI projectSubKPI)
        {
            var entity = new ProjectSubKPI
            {
                ProjectSubKPIId = projectSubKPI.ProjectSubKPIId,
                SubKPIName = projectSubKPI.SubKPIName,
                Unit = projectSubKPI.Unit,
                KPIType = projectSubKPI.KPIType,
                Description = projectSubKPI.Description,
                ProjectKPIId = projectSubKPI.ProjectKPIId
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
