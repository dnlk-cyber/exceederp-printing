﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintLaborRateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintLabourRates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintLabourRate> printlabourrates = db.PrintLabourRates;
            DataSourceResult result = printlabourrates.ToDataSourceResult(request, printLabourRate => new {
                PrintLaborRateId = printLabourRate.PrintLaborRateId,
                PeriodCode = printLabourRate.PeriodCode,
                PeriodName = printLabourRate.PeriodName,
                NormalHourRate = printLabourRate.NormalHourRate,
                OverTimeHourRate = printLabourRate.OverTimeHourRate,
                OverHeadRate = printLabourRate.OverHeadRate,
                DateCreated = printLabourRate.DateCreated,
                LastModified = printLabourRate.LastModified,
                CreatedBy = printLabourRate.CreatedBy,
                ModifiedBy = printLabourRate.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintLabourRates_Create([DataSourceRequest]DataSourceRequest request, PrintLabourRate printLabourRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintLabourRate
                {
                    PeriodCode = printLabourRate.PeriodCode,
                    PeriodName = printLabourRate.PeriodName,
                    NormalHourRate = printLabourRate.NormalHourRate,
                    OverTimeHourRate = printLabourRate.OverTimeHourRate,
                    OverHeadRate = printLabourRate.OverHeadRate,
                    DateCreated = printLabourRate.DateCreated,
                    LastModified = printLabourRate.LastModified,
                    CreatedBy = printLabourRate.CreatedBy,
                    ModifiedBy = printLabourRate.ModifiedBy
                };

                db.PrintLabourRates.Add(entity);
                db.SaveChanges();
                printLabourRate.PrintLaborRateId = entity.PrintLaborRateId;
            }

            return Json(new[] { printLabourRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintLabourRates_Update([DataSourceRequest]DataSourceRequest request, PrintLabourRate printLabourRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintLabourRate
                {
                    PrintLaborRateId = printLabourRate.PrintLaborRateId,
                    PeriodCode = printLabourRate.PeriodCode,
                    PeriodName = printLabourRate.PeriodName,
                    NormalHourRate = printLabourRate.NormalHourRate,
                    OverTimeHourRate = printLabourRate.OverTimeHourRate,
                    OverHeadRate = printLabourRate.OverHeadRate,
                    DateCreated = printLabourRate.DateCreated,
                    LastModified = printLabourRate.LastModified,
                    CreatedBy = printLabourRate.CreatedBy,
                    ModifiedBy = printLabourRate.ModifiedBy
                };

                db.PrintLabourRates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printLabourRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintLabourRates_Destroy([DataSourceRequest]DataSourceRequest request, PrintLabourRate printLabourRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintLabourRate
                {
                    PrintLaborRateId = printLabourRate.PrintLaborRateId,
                    PeriodCode = printLabourRate.PeriodCode,
                    PeriodName = printLabourRate.PeriodName,
                    NormalHourRate = printLabourRate.NormalHourRate,
                    OverTimeHourRate = printLabourRate.OverTimeHourRate,
                    OverHeadRate = printLabourRate.OverHeadRate,
                    DateCreated = printLabourRate.DateCreated,
                    LastModified = printLabourRate.LastModified,
                    CreatedBy = printLabourRate.CreatedBy,
                    ModifiedBy = printLabourRate.ModifiedBy
                };

                db.PrintLabourRates.Attach(entity);
                db.PrintLabourRates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printLabourRate }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
