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
    public class ProjectMonthlyPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectMonthlyPlans_Read([DataSourceRequest]DataSourceRequest request,int pMId)
        {
            IQueryable<ProjectMonthlyPlan> projectmonthlyplans = db.ProjectMonthlyPlans.Where(x=>x.ProjectPlanId==pMId);
            DataSourceResult result = projectmonthlyplans.ToDataSourceResult(request, projectMonthlyPlan => new {
                ProjectMonthlyPlanId = projectMonthlyPlan.ProjectMonthlyPlanId,
                GLPeriodId = projectMonthlyPlan.GLPeriodId,
                ProjectPlanId = projectMonthlyPlan.ProjectPlanId,
                Value = projectMonthlyPlan.Value,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMonthlyPlans_Update([DataSourceRequest]DataSourceRequest request, ProjectMonthlyPlan projectMonthlyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMonthlyPlan
                {
                    ProjectMonthlyPlanId = projectMonthlyPlan.ProjectMonthlyPlanId,
                    ProjectPlanId = projectMonthlyPlan.ProjectPlanId,
                    GLPeriodId = projectMonthlyPlan.GLPeriodId,
                    Value = projectMonthlyPlan.Value,
                };

                db.ProjectMonthlyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMonthlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProjectMonthlyPlan projectMonthlyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMonthlyPlan
                {
                    ProjectMonthlyPlanId = projectMonthlyPlan.ProjectMonthlyPlanId,
                    GLPeriodId = projectMonthlyPlan.GLPeriodId,
                    Value = projectMonthlyPlan.Value,
                };

                db.ProjectMonthlyPlans.Attach(entity);
                db.ProjectMonthlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectMonthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
