﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Planning.Project;

namespace ExceedERP.Web.Areas.Planning.Controllers.Project
{
    public class ProjectWeeklyPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectWeeklyPlans_Read([DataSourceRequest]DataSourceRequest request,int mpId)
        {
            IQueryable<ProjectWeeklyPlan> weeklyplans = db.ProjectWeeklyPlans.Where(x=>x.ProjectMonthlyPlanId==mpId);
            DataSourceResult result = weeklyplans.ToDataSourceResult(request, weeklyPlan => new {
                ProjectWeeklyPlanId = weeklyPlan.ProjectWeeklyPlanId,
                WeekOne = weeklyPlan.WeekOne,
                WeekTwo = weeklyPlan.WeekTwo,
                WeekThree = weeklyPlan.WeekThree,
                WeekFour = weeklyPlan.WeekFour,
                ProjectMonthlyPlanId = weeklyPlan.ProjectMonthlyPlanId

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyPlans_Create([DataSourceRequest]DataSourceRequest request, ProjectWeeklyPlan weeklyPlan,int mpId)
        {
            if (ModelState.IsValid)
            {
                weeklyPlan.ProjectMonthlyPlanId = mpId;
                var entity = GetMappedObject(weeklyPlan);
                db.ProjectWeeklyPlans.Add(entity);
                db.SaveChanges();
                weeklyPlan.ProjectWeeklyPlanId = entity.ProjectWeeklyPlanId;
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyPlans_Update([DataSourceRequest]DataSourceRequest request, ProjectWeeklyPlan weeklyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyPlan);
                db.ProjectWeeklyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyPlans_Destroy([DataSourceRequest]DataSourceRequest request, ProjectWeeklyPlan weeklyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyPlan);
                db.ProjectWeeklyPlans.Attach(entity);
                db.ProjectWeeklyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }
        private ProjectWeeklyPlan GetMappedObject(ProjectWeeklyPlan weeklyPlan)
        {
            var entity = new ProjectWeeklyPlan
            {
                ProjectWeeklyPlanId = weeklyPlan.ProjectWeeklyPlanId,
                WeekOne = weeklyPlan.WeekOne,
                WeekTwo = weeklyPlan.WeekTwo,
                WeekThree = weeklyPlan.WeekThree,
                WeekFour = weeklyPlan.WeekFour,
                ProjectMonthlyPlanId = weeklyPlan.ProjectMonthlyPlanId

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
