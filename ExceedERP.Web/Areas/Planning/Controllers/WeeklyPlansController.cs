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
    [Authorize]
    public class WeeklyPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly ExceedERP.Helpers.Planning.PlanningApprovalHelper auth;

        private const string NotEditableMessage =
            "You can change figures only on your own work unit's plan, and only while it is a draft or has been returned for revision.";

        public WeeklyPlansController()
        {
            auth = new ExceedERP.Helpers.Planning.PlanningApprovalHelper(db);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WeeklyPlans_Read([DataSourceRequest]DataSourceRequest request,int mpId)
        {
            if (!CanViewMonthlyPlan(mpId))
                return Json(Enumerable.Empty<WeeklyPlan>().ToDataSourceResult(request));

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
        [ValidateAntiForgeryToken]
        public ActionResult WeeklyPlans_Create([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan,int mpId)
        {
            weeklyPlan.ActualWeekOne = 0;
            weeklyPlan.ActualWeekTwo = 0;
            weeklyPlan.ActualWeekThree = 0;
            weeklyPlan.ActualWeekFour = 0;
            weeklyPlan.MonthlyPlanId = mpId;
            var monthlyPlan = db.MonthlyPlans.Find(mpId);
            if (monthlyPlan == null)
                ModelState.AddModelError(string.Empty, "The monthly plan was not found.");
            else if (IsPlanLocked(monthlyPlan.PlanningDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);
          
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyPlan);
                db.WeeklyPlans.Add(entity);
                if (monthlyPlan != null)
                {
                    monthlyPlan.Value += weeklyPlan.WeekOne + weeklyPlan.WeekTwo + weeklyPlan.WeekThree + weeklyPlan.WeekFour;
                    db.Entry(monthlyPlan).State = EntityState.Modified;
                }
                db.SaveChanges();
                weeklyPlan.WeeklyPlanId = entity.WeeklyPlanId;
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult WeeklyPlans_Update([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan)
        {
            weeklyPlan.ActualWeekOne = 0;
            weeklyPlan.ActualWeekTwo = 0;
            weeklyPlan.ActualWeekThree = 0;
            weeklyPlan.ActualWeekFour = 0;
            var entity = db.WeeklyPlans.Find(weeklyPlan.WeeklyPlanId);
            var monthlyPlan = entity == null ? null : db.MonthlyPlans.Find(entity.MonthlyPlanId);
            if (entity == null || monthlyPlan == null)
                ModelState.AddModelError(string.Empty, "The weekly or monthly plan was not found.");
            else if (IsPlanLocked(monthlyPlan.PlanningDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                entity.WeekOne = weeklyPlan.WeekOne;
                entity.WeekTwo = weeklyPlan.WeekTwo;
                entity.WeekThree = weeklyPlan.WeekThree;
                entity.WeekFour = weeklyPlan.WeekFour;
                entity.ActualWeekOne = 0;
                entity.ActualWeekTwo = 0;
                entity.ActualWeekThree = 0;
                entity.ActualWeekFour = 0;
                db.SaveChanges();
                if (monthlyPlan != null)
                {
                    var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyPlan.MonthlyPlanId).ToList();
                    if (weeklyPlans.Any())
                    {
                        monthlyPlan.Value = weeklyPlans.Sum(x=>x.WeekOne) + weeklyPlans.Sum(x => x.WeekTwo) + weeklyPlans.Sum(x => x.WeekThree) + weeklyPlans.Sum(x => x.WeekFour);
                        db.Entry(monthlyPlan).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
              

            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult WeeklyPlans_Destroy([DataSourceRequest]DataSourceRequest request, WeeklyPlan weeklyPlan)
        {
            var entity = db.WeeklyPlans.Find(weeklyPlan.WeeklyPlanId);
            var monthlyPlan = entity == null ? null : db.MonthlyPlans.Find(entity.MonthlyPlanId);
            if (entity == null || monthlyPlan == null)
                ModelState.AddModelError(string.Empty, "The weekly or monthly plan was not found.");
            else if (IsPlanLocked(monthlyPlan.PlanningDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
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

        private bool IsPlanLocked(int planningDetailId)
        {
            var plan = auth.GetPlanOfDetail(planningDetailId);
            var placement = auth.GetCurrentPlacement(ExceedERP.Web.Helpers.UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return !auth.CanEditFigures(plan, placement);
        }

        private bool CanViewMonthlyPlan(int monthlyPlanId)
        {
            var plan = auth.GetPlanOfMonthlyPlan(monthlyPlanId);
            if (plan != null && PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return true;

            var placement = auth.GetCurrentPlacement(ExceedERP.Web.Helpers.UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return plan != null && placement != null && auth.CanViewPlan(plan, placement);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
