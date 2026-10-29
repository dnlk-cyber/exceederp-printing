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
    public class WeeklyActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WeeklyActuals_Read([DataSourceRequest]DataSourceRequest request,int wAId)
        {
            IQueryable<WeeklyActual> weeklyactuals = db.WeeklyActuals.Where(x=>x.MonthlyActualId== wAId);
            DataSourceResult result = weeklyactuals.ToDataSourceResult(request, weeklyActual => new {
                WeeklyActualId = weeklyActual.WeeklyActualId,
                MonthlyActualId = weeklyActual.MonthlyActualId,
                WeekOne = weeklyActual.WeekOne,
                WeekTwo = weeklyActual.WeekTwo,
                WeekThree = weeklyActual.WeekThree,
                WeekFour = weeklyActual.WeekFour,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyActuals_Create([DataSourceRequest]DataSourceRequest request, WeeklyActual weeklyActual,int wAId)
        {
            var monthlyActual = db.MonthlyActuals.Find(wAId);

            if (ModelState.IsValid)
            {

                weeklyActual.MonthlyActualId = wAId;
                var entity = GetMappedObject(weeklyActual);
                db.WeeklyActuals.Add(entity);
                if (monthlyActual != null)
                {
                    monthlyActual.ActualValue += weeklyActual.WeekOne + weeklyActual.WeekTwo + weeklyActual.WeekThree + weeklyActual.WeekFour;
                    db.Entry(monthlyActual).State = EntityState.Modified;
                }
                db.SaveChanges();
                weeklyActual.WeeklyActualId = entity.WeeklyActualId;
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyActuals_Update([DataSourceRequest]DataSourceRequest request, WeeklyActual weeklyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyActual);
                db.WeeklyActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var monthlyActual = db.MonthlyActuals.Find(weeklyActual.MonthlyActualId);
                if (monthlyActual != null)
                {
                    var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == monthlyActual.MonthlyActualId).ToList();
                    if (weeklyActuals.Any())
                    {
                        monthlyActual.ActualValue = weeklyActuals.Sum(x => x.WeekOne) + weeklyActuals.Sum(x => x.WeekTwo) + weeklyActuals.Sum(x => x.WeekThree) + weeklyActuals.Sum(x => x.WeekFour);
                        db.Entry(monthlyActual).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyActuals_Destroy([DataSourceRequest]DataSourceRequest request, WeeklyActual weeklyActual)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weeklyActual);
                db.WeeklyActuals.Attach(entity);
                db.WeeklyActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weeklyActual }.ToDataSourceResult(request, ModelState));
        }
        private WeeklyActual GetMappedObject(WeeklyActual weeklyActual)
        {
            var entity = new WeeklyActual
            {
                WeeklyActualId = weeklyActual.WeeklyActualId,
                MonthlyActualId = weeklyActual.MonthlyActualId,
                WeekOne = weeklyActual.WeekOne,
                WeekTwo = weeklyActual.WeekTwo,
                WeekThree = weeklyActual.WeekThree,
                WeekFour = weeklyActual.WeekFour,
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
