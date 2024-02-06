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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting
{
    public class FurnitureJobOrderTimeSheetsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobOrderTimeSheets_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobOrderTimeSheet> jobordertimesheets = db.FurnitureJobOrderTimeSheets.Where(x=>x.FurnitureJobOrderTimeRegistrationId == id);
            DataSourceResult result = jobordertimesheets.ToDataSourceResult(request, jobOrderTimeSheet => new {
                JobOrderTimeSheetId = jobOrderTimeSheet.FurnitureJobOrderTimeSheetId,
                JobOrderTimeRegistrationId = jobOrderTimeSheet.FurnitureJobOrderTimeRegistrationId,
                Date = jobOrderTimeSheet.Date,
                Work = jobOrderTimeSheet.Work,
                StartTime = jobOrderTimeSheet.StartTime,
                ComplationTime = jobOrderTimeSheet.ComplationTime,
                TotalTime = jobOrderTimeSheet.TotalTime,
                NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                Remark = jobOrderTimeSheet.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeSheets_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeSheet jobOrderTimeSheet, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeSheet
                {
                    FurnitureJobOrderTimeRegistrationId = id,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.FurnitureJobOrderTimeSheets.Add(entity);
                db.SaveChanges();
                jobOrderTimeSheet.FurnitureJobOrderTimeSheetId = entity.FurnitureJobOrderTimeSheetId;
            }

            return Json(new[] { jobOrderTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeSheets_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeSheet jobOrderTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeSheet
                {
                    FurnitureJobOrderTimeSheetId = jobOrderTimeSheet.FurnitureJobOrderTimeSheetId,
                    FurnitureJobOrderTimeRegistrationId = jobOrderTimeSheet.FurnitureJobOrderTimeRegistrationId,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.FurnitureJobOrderTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeSheets_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeSheet jobOrderTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeSheet
                {
                    FurnitureJobOrderTimeSheetId = jobOrderTimeSheet.FurnitureJobOrderTimeSheetId,
                    FurnitureJobOrderTimeRegistrationId = jobOrderTimeSheet.FurnitureJobOrderTimeRegistrationId,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.FurnitureJobOrderTimeSheets.Attach(entity);
                db.FurnitureJobOrderTimeSheets.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
