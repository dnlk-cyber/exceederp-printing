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
    public class ProjectWeeklyActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectWeeklyActuals_Read([DataSourceRequest]DataSourceRequest request,int wId)
        {
            IQueryable<ProjectWeeklyActual> weeklyactuals = db.ProjectWeeklyActuals.Where(x=>x.ProjectMonthlyActualId==wId);
            DataSourceResult result = weeklyactuals.ToDataSourceResult(request, weeklyActual => new {
                ProjectWeeklyActualId = weeklyActual.ProjectWeeklyActualId,
                WeekOne = weeklyActual.WeekOne,
                WeekTwo = weeklyActual.WeekTwo,
                WeekThree = weeklyActual.WeekThree,
                WeekFour = weeklyActual.WeekFour,
                ProjectMonthlyActualId = weeklyActual.ProjectMonthlyActualId


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyActuals_Create([DataSourceRequest]DataSourceRequest request, ProjectWeeklyActual weeklyActual,int wId)
        {
            if (ModelState.IsValid)
            {
                weeklyActual.ProjectMonthlyActualId = wId;
                var entity = GetMappedObject(weeklyActual);

                db.ProjectWeeklyActuals.Add(entity);
                db.SaveChanges();
                weeklyActual.ProjectWeeklyActualId = entity.ProjectWeeklyActualId;
                var projectMonthlyActual = db.ProjectMonthlyActuals.Find(wId);
                if(projectMonthlyActual != null)
                {
                    projectMonthlyActual.ActualValue = weeklyActual.WeekOne + weeklyActual.WeekTwo + weeklyActual.WeekThree + weeklyActual.WeekFour;
                    db.ProjectMonthlyActuals.Attach(projectMonthlyActual);
                    db.Entry(projectMonthlyActual).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyActuals_Update([DataSourceRequest]DataSourceRequest request, ProjectWeeklyActual weeklyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyActual);

                db.ProjectWeeklyActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var projectMonthlyActual = db.ProjectMonthlyActuals.Find(weeklyActual.ProjectMonthlyActualId);
                if (projectMonthlyActual != null)
                {
                    projectMonthlyActual.ActualValue = weeklyActual.WeekOne + weeklyActual.WeekTwo + weeklyActual.WeekThree + weeklyActual.WeekFour;
                    db.ProjectMonthlyActuals.Attach(projectMonthlyActual);
                    db.Entry(projectMonthlyActual).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWeeklyActuals_Destroy([DataSourceRequest]DataSourceRequest request, ProjectWeeklyActual weeklyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyActual);
                db.ProjectWeeklyActuals.Attach(entity);
                db.ProjectWeeklyActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }
        private ProjectWeeklyActual GetMappedObject(ProjectWeeklyActual weeklyPlan)
        {
            var entity = new ProjectWeeklyActual
            {
                ProjectWeeklyActualId = weeklyPlan.ProjectWeeklyActualId,
                WeekOne = weeklyPlan.WeekOne,
                WeekTwo = weeklyPlan.WeekTwo,
                WeekThree = weeklyPlan.WeekThree,
                WeekFour = weeklyPlan.WeekFour,
                ProjectMonthlyActualId = weeklyPlan.ProjectMonthlyActualId

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
