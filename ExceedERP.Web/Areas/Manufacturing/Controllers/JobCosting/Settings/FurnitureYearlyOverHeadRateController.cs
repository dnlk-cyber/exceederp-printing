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
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting.Settings
{
    //[AuthorizeRoles(JobCostingRoles.JobCostingAdmin)]
    public class FurnitureYearlyOverHeadRateController : Controller
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
            IQueryable<FurnitureYearlyOverHeadRate> yearlyoverheadrates = db.FurnitureYearlyOverHeadRates;
            DataSourceResult result = yearlyoverheadrates.ToDataSourceResult(request, yearlyOverHeadRate => new {
                FurnitureYearlyOverHeadRateId = yearlyOverHeadRate.FurnitureYearlyOverHeadRateId,
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

        public ActionResult YearlyOverHeadRates_Create([DataSourceRequest]DataSourceRequest request, FurnitureYearlyOverHeadRate yearlyOverHeadRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureYearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);
                db.FurnitureYearlyOverHeadRates.Add(entity);
                db.SaveChanges();
                yearlyOverHeadRate.FurnitureYearlyOverHeadRateId = entity.FurnitureYearlyOverHeadRateId;
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyOverHeadRates_Update([DataSourceRequest]DataSourceRequest request, FurnitureYearlyOverHeadRate yearlyOverHeadRate)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureYearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);

                db.FurnitureYearlyOverHeadRates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyOverHeadRates_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureYearlyOverHeadRate yearlyOverHeadRate)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureYearlyOverHeadRate)this.GetObject(yearlyOverHeadRate);

                db.FurnitureYearlyOverHeadRates.Attach(entity);
                db.FurnitureYearlyOverHeadRates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { yearlyOverHeadRate }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureYearlyOverHeadRate yearlyOverHeadRate)
        {
            var entity = new FurnitureYearlyOverHeadRate
            {
                FurnitureYearlyOverHeadRateId = yearlyOverHeadRate.FurnitureYearlyOverHeadRateId,
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