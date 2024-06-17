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
    public class ProjectObjectivesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectObjectives_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectObjective> projectobjectives = db.ProjectObjectives;
            DataSourceResult result = projectobjectives.ToDataSourceResult(request, projectObjective => new {
                ProjectObjectiveId = projectObjective.ProjectObjectiveId,
                ProjectObjectiveName = projectObjective.ProjectObjectiveName,
                Description = projectObjective.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectObjectives_Create([DataSourceRequest]DataSourceRequest request, ProjectObjective projectObjective)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectObjective
                {
                    ProjectObjectiveName = projectObjective.ProjectObjectiveName,
                    Description = projectObjective.Description
                };

                db.ProjectObjectives.Add(entity);
                db.SaveChanges();
                projectObjective.ProjectObjectiveId = entity.ProjectObjectiveId;
            }

            return Json(new[] { projectObjective }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectObjectives_Update([DataSourceRequest]DataSourceRequest request, ProjectObjective projectObjective)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectObjective
                {
                    ProjectObjectiveId = projectObjective.ProjectObjectiveId,
                    ProjectObjectiveName = projectObjective.ProjectObjectiveName,
                    Description = projectObjective.Description
                };

                db.ProjectObjectives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectObjective }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectObjectives_Destroy([DataSourceRequest]DataSourceRequest request, ProjectObjective projectObjective)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectObjective
                {
                    ProjectObjectiveId = projectObjective.ProjectObjectiveId,
                    ProjectObjectiveName = projectObjective.ProjectObjectiveName,
                    Description = projectObjective.Description
                };

                db.ProjectObjectives.Attach(entity);
                db.ProjectObjectives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectObjective }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
