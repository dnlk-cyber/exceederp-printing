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
using ExceedERP.Core.Domain.Planning;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class MonthlyActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyActuals_Read([DataSourceRequest]DataSourceRequest request,int mAId)
        {
            IQueryable<MonthlyActual> monthlyactuals = db.MonthlyActuals.Where(x=>x.ActualDetailId == mAId);
            DataSourceResult result = monthlyactuals.ToDataSourceResult(request, monthlyActual => new {
                MonthlyActualId = monthlyActual.MonthlyActualId,
                ActualDetailId = monthlyActual.ActualDetailId,
                GLPeriodId =  monthlyActual.GLPeriodId,
                PlanValue =   monthlyActual.PlanValue,
                ActualValue = monthlyActual.ActualValue
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyActuals_Create([DataSourceRequest]DataSourceRequest request, MonthlyActual monthlyActual,int mAId)
        {
            if (ModelState.IsValid)
            {
                monthlyActual.ActualDetailId = mAId;
                var entity = GetMappedObject(monthlyActual);
                db.MonthlyActuals.Add(entity);
                db.SaveChanges();
                monthlyActual.MonthlyActualId = entity.MonthlyActualId;
            }

            return Json(new[] { monthlyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyActuals_Update([DataSourceRequest]DataSourceRequest request, MonthlyActual monthlyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(monthlyActual);
                db.MonthlyActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { monthlyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyActuals_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyActual monthlyActual)
        {
            var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == monthlyActual.MonthlyActualId).ToList();
            if (weeklyActuals.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(monthlyActual);
                db.MonthlyActuals.Attach(entity);
                db.MonthlyActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { monthlyActual }.ToDataSourceResult(request, ModelState));
        }
        private MonthlyActual GetMappedObject(MonthlyActual monthlyActual)
        {
            var entity = new MonthlyActual
            {
                MonthlyActualId = monthlyActual.MonthlyActualId,
                ActualDetailId = monthlyActual.ActualDetailId,
                GLPeriodId = monthlyActual.GLPeriodId,
                PlanValue = monthlyActual.PlanValue,
                ActualValue = monthlyActual.ActualValue
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
