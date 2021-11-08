﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class LeaveNewAwajStartYearsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FisicalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult LeaveNewAwajStartYears_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeaveNewAwajStartYear> leavenewawajstartyears = db.LeaveNewAwajStartYears;
            DataSourceResult result = leavenewawajstartyears.ToDataSourceResult(request, leaveNewAwajStartYear => new {
                LeaveNewAwajStartYearId = leaveNewAwajStartYear.LeaveNewAwajStartYearId,
                GlFisicalYearId = leaveNewAwajStartYear.GlFisicalYearId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveNewAwajStartYears_Create([DataSourceRequest]DataSourceRequest request, LeaveNewAwajStartYear leaveNewAwajStartYear)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveNewAwajStartYear
                {
                    GlFisicalYearId = leaveNewAwajStartYear.GlFisicalYearId
                };

                db.LeaveNewAwajStartYears.Add(entity);
                db.SaveChanges();
                leaveNewAwajStartYear.LeaveNewAwajStartYearId = entity.LeaveNewAwajStartYearId;
            }

            return Json(new[] { leaveNewAwajStartYear }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveNewAwajStartYears_Update([DataSourceRequest]DataSourceRequest request, LeaveNewAwajStartYear leaveNewAwajStartYear)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveNewAwajStartYear
                {
                    LeaveNewAwajStartYearId = leaveNewAwajStartYear.LeaveNewAwajStartYearId,
                    GlFisicalYearId = leaveNewAwajStartYear.GlFisicalYearId
                };

                db.LeaveNewAwajStartYears.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { leaveNewAwajStartYear }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveNewAwajStartYears_Destroy([DataSourceRequest]DataSourceRequest request, LeaveNewAwajStartYear leaveNewAwajStartYear)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveNewAwajStartYear
                {
                    LeaveNewAwajStartYearId = leaveNewAwajStartYear.LeaveNewAwajStartYearId,
                    GlFisicalYearId = leaveNewAwajStartYear.GlFisicalYearId
                };

                db.LeaveNewAwajStartYears.Attach(entity);
                db.LeaveNewAwajStartYears.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leaveNewAwajStartYear }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
