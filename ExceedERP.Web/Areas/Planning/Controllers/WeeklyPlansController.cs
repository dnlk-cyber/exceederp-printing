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

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    public class WeeklyPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WeeklyPlans_Read([DataSourceRequest]DataSourceRequest request,int mpId)
        {
            IQueryable<WeeklyPlan> weeklyplans = db.WeeklyPlans.Where(x=>x.MonthlyPlanId==mpId);
            DataSourceResult result = weeklyplans.ToDataSourceResult(request, weeklyPlan => new {
                WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                WeekOne = weeklyPlan.WeekOne,
                WeekTwo = weeklyPlan.WeekTwo,
                WeekThree = weeklyPlan.WeekThree,
                WeekFour = weeklyPlan.WeekFour,
                ActualWeekOne = weeklyPlan.ActualWeekOne,
                ActualWeekTwo = weeklyPlan.ActualWeekTwo,
                ActualWeekThree = weeklyPlan.ActualWeekThree,
                ActualWeekFour = weeklyPlan.ActualWeekFour,
                MonthlyPlanId = weeklyPlan.MonthlyPlanId

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyPlans_Create([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan,int mpId)
        {
            weeklyPlan.ActualWeekOne = 0;
            weeklyPlan.ActualWeekTwo = 0;
            weeklyPlan.ActualWeekThree = 0;
            weeklyPlan.ActualWeekFour = 0;
            if (ModelState.IsValid)
            {
                weeklyPlan.MonthlyPlanId = mpId;
                var entity = GetMappedObject(weeklyPlan);
                db.WeeklyPlans.Add(entity);
                db.SaveChanges();
                weeklyPlan.WeeklyPlanId = entity.WeeklyPlanId;
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyPlans_Update([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan)
        {
            weeklyPlan.ActualWeekOne = 0;
            weeklyPlan.ActualWeekTwo = 0;
            weeklyPlan.ActualWeekThree = 0;
            weeklyPlan.ActualWeekFour = 0;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyPlan);
                db.WeeklyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyPlans_Destroy([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyPlan);
                db.WeeklyPlans.Attach(entity);
                db.WeeklyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }
        private WeeklyPlan GetMappedObject(WeeklyPlan weeklyPlan)
        {
            var entity = new WeeklyPlan
            {
                WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                WeekOne = weeklyPlan.WeekOne,
                WeekTwo = weeklyPlan.WeekTwo,
                WeekThree = weeklyPlan.WeekThree,
                WeekFour = weeklyPlan.WeekFour,
                ActualWeekOne = weeklyPlan.ActualWeekOne,
                ActualWeekTwo = weeklyPlan.ActualWeekTwo,
                ActualWeekThree = weeklyPlan.ActualWeekThree,
                ActualWeekFour = weeklyPlan.ActualWeekFour,
                MonthlyPlanId = weeklyPlan.MonthlyPlanId

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
