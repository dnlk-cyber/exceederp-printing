﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingDailyLaborRate)]
    public class DailyLaborRateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            var glsegmentId = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter)?.GLSegmentSetupId ?? 0;

            var costcenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glsegmentId).AsQueryable();
            ViewData["CostCenters"] = costcenters.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            return View();
        }

        public ActionResult DailyLaborRates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DailyLaborRate> dailylaborrates = db.DailyLaborRates;
            DataSourceResult result = dailylaborrates.ToDataSourceResult(request, dailyLaborRate => new {
                DailyLaborRateId = dailyLaborRate.DailyLaborRateId,
                CostCenter = dailyLaborRate.CostCenter,
                GlFiscalYearId = dailyLaborRate.GlFiscalYearId,
                GLPeriodId = dailyLaborRate.GLPeriodId,
                TotalSalary = dailyLaborRate.TotalSalary,
                TotalExpectedHour = dailyLaborRate.TotalExpectedHour,
                DLR = dailyLaborRate.DLR,
                EmployeeType = dailyLaborRate.EmployeeType,
                DateCreated = dailyLaborRate.DateCreated,
                LastModified = dailyLaborRate.LastModified,
                CreatedBy = dailyLaborRate.CreatedBy,
                ModifiedBy = dailyLaborRate.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult DailyLaborRates_Create([DataSourceRequest]DataSourceRequest request, DailyLaborRate dailyLaborRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (DailyLaborRate)this.GetObject(dailyLaborRate);
                
                db.DailyLaborRates.Add(entity);
                db.SaveChanges();
                dailyLaborRate.DailyLaborRateId = entity.DailyLaborRateId;
                return Json(new[] { entity }.ToDataSourceResult(request, ModelState));

            }

            return Json(new[] { dailyLaborRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyLaborRates_Update([DataSourceRequest]DataSourceRequest request, DailyLaborRate dailyLaborRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (DailyLaborRate)this.GetObject(dailyLaborRate);

                db.DailyLaborRates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dailyLaborRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyLaborRates_Destroy([DataSourceRequest]DataSourceRequest request, DailyLaborRate dailyLaborRate)
        {

            if (ModelState.IsValid)
            {
                var entity = (DailyLaborRate)this.GetObject(dailyLaborRate);

                db.DailyLaborRates.Attach(entity);
                db.DailyLaborRates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dailyLaborRate }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(DailyLaborRate dailyLaborRate)
        {
           
            var entity = new DailyLaborRate
            {
                DailyLaborRateId = dailyLaborRate.DailyLaborRateId,
                CostCenter = dailyLaborRate.CostCenter,
                GlFiscalYearId = dailyLaborRate.GlFiscalYearId,
                GLPeriodId = dailyLaborRate.GLPeriodId,
                TotalSalary = dailyLaborRate.TotalSalary,
                TotalExpectedHour = dailyLaborRate.TotalExpectedHour,
                DLR = dailyLaborRate.DLR,
                EmployeeType = dailyLaborRate.EmployeeType,
                DateCreated = dailyLaborRate.DateCreated,
                LastModified = dailyLaborRate.LastModified,
                CreatedBy = dailyLaborRate.CreatedBy,
                ModifiedBy = dailyLaborRate.ModifiedBy

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