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
    [Authorize]
    public class ProjectMonthlyActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectMonthlyActuals_Read([DataSourceRequest]DataSourceRequest request,int aId)
        {
            IQueryable<ProjectMonthlyActual> projectmonthlyactuals = db.ProjectMonthlyActuals.Where(x=>x.ProjectActualId ==aId);
            DataSourceResult result = projectmonthlyactuals.ToDataSourceResult(request, projectMonthlyActual => new {
                ProjectMonthlyActualId = projectMonthlyActual.ProjectMonthlyActualId,
                ProjectActualId = projectMonthlyActual.ProjectActualId,
                GLPeriodId = projectMonthlyActual.GLPeriodId,
                PlanValue = projectMonthlyActual.PlanValue,
                ActualValue = projectMonthlyActual.ActualValue,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMonthlyActuals_Create([DataSourceRequest]DataSourceRequest request, ProjectMonthlyActual projectMonthlyActual,int aId)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new ProjectMonthlyActual
                {
                    GLPeriodId = projectMonthlyActual.GLPeriodId,
                    ProjectActualId = projectMonthlyActual.ProjectActualId,
                    PlanValue = projectMonthlyActual.PlanValue,
                    ActualValue = projectMonthlyActual.ActualValue,
                };

                db.ProjectMonthlyActuals.Add(entity);
                db.SaveChanges();
                projectMonthlyActual.ProjectMonthlyActualId = entity.ProjectMonthlyActualId;
            }

            return Json(new[] { projectMonthlyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMonthlyActuals_Update([DataSourceRequest]DataSourceRequest request, ProjectMonthlyActual projectMonthlyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMonthlyActual
                {
                    ProjectMonthlyActualId = projectMonthlyActual.ProjectMonthlyActualId,
                    ProjectActualId = projectMonthlyActual.ProjectActualId,
                    GLPeriodId = projectMonthlyActual.GLPeriodId,
                    PlanValue = projectMonthlyActual.PlanValue,
                    ActualValue = projectMonthlyActual.ActualValue,
                };

                db.ProjectMonthlyActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectMonthlyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMonthlyActuals_Destroy([DataSourceRequest]DataSourceRequest request, ProjectMonthlyActual projectMonthlyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMonthlyActual
                {
                    ProjectMonthlyActualId = projectMonthlyActual.ProjectMonthlyActualId,
                    ProjectActualId = projectMonthlyActual.ProjectActualId,
                    GLPeriodId = projectMonthlyActual.GLPeriodId,
                    PlanValue = projectMonthlyActual.PlanValue,
                    ActualValue = projectMonthlyActual.ActualValue,
                };

                db.ProjectMonthlyActuals.Attach(entity);
                db.ProjectMonthlyActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectMonthlyActual }.ToDataSourceResult(request, ModelState));
        }
        private int PUtProjectMonthlyActual(int actualId, int fiscalYearId)
        {
            var glPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();

            foreach (var period in glPeriods)
            {
                //var isMonthlyPlanExist = db.ProjectMonthlyPlans.Where(x=>x.ProjectPlanId==planId).Any();
                var entity = new ProjectMonthlyActual
                {
                    ProjectActualId = actualId,
                    GLPeriodId = period.GLPeriodId,
                    PlanValue = 0,
                    ActualValue = 0,
                };

                db.ProjectMonthlyActuals.Add(entity);
                db.SaveChanges();
            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
