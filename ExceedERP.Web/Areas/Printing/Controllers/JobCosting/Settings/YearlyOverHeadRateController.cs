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
using ExceedERP.Core.Domain.printing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting.Settings
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingAdmin)]
    public class YearlyOverHeadRateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            var glsegmentId = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter)?.GLSegmentSetupId ?? 0;

            var costcenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glsegmentId).AsQueryable();
            ViewData["CostCenters"] = costcenters.ToList();
            return View();
        }

        public ActionResult YearlyOverHeadRates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<YearlyOverHeadRate> yearlyoverheadrates = db.YearlyOverHeadRates;
            DataSourceResult result = yearlyoverheadrates.ToDataSourceResult(request, yearlyOverHeadRate => new {
                YearlyOverHeadRateId = yearlyOverHeadRate.YearlyOverHeadRateId,
                GlFiscalYearId = yearlyOverHeadRate.GlFiscalYearId,
                Values = yearlyOverHeadRate.Values,
                Rate = yearlyOverHeadRate.Rate,
                Description = yearlyOverHeadRate.Description,
                DateCreated = yearlyOverHeadRate.DateCreated,
                LastModified = yearlyOverHeadRate.LastModified,
                CreatedBy = yearlyOverHeadRate.CreatedBy,
                ModifiedBy = yearlyOverHeadRate.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult YearlyOverHeadRates_Create([DataSourceRequest]DataSourceRequest request, YearlyOverHeadRate yearlyOverHeadRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (YearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);
                db.YearlyOverHeadRates.Add(entity);
                db.SaveChanges();
                yearlyOverHeadRate.YearlyOverHeadRateId = entity.YearlyOverHeadRateId;
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyOverHeadRates_Update([DataSourceRequest]DataSourceRequest request, YearlyOverHeadRate yearlyOverHeadRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (YearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);

                db.YearlyOverHeadRates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyOverHeadRates_Destroy([DataSourceRequest]DataSourceRequest request, YearlyOverHeadRate yearlyOverHeadRate)
        {

            if (ModelState.IsValid)
            {
                var entity = (YearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);

                db.YearlyOverHeadRates.Attach(entity);
                db.YearlyOverHeadRates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(YearlyOverHeadRate yearlyOverHeadRate)
        {
            var entity = new YearlyOverHeadRate
            {
                YearlyOverHeadRateId = yearlyOverHeadRate.YearlyOverHeadRateId,
                GlFiscalYearId = yearlyOverHeadRate.GlFiscalYearId,
                Values = yearlyOverHeadRate.Values,
                Rate = yearlyOverHeadRate.Rate,
                Description = yearlyOverHeadRate.Description,
                DateCreated = yearlyOverHeadRate.DateCreated,
                LastModified = yearlyOverHeadRate.LastModified,
                CreatedBy = yearlyOverHeadRate.CreatedBy,
                ModifiedBy = yearlyOverHeadRate.ModifiedBy

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