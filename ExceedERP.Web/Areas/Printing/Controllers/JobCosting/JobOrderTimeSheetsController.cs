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
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting
{
    public class JobOrderTimeSheetsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobOrderTimeSheets_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobOrderTimeSheet> jobordertimesheets = db.JobOrderTimeSheets.Where(x=>x.JobOrderTimeRegistrationId == id);
            DataSourceResult result = jobordertimesheets.ToDataSourceResult(request, jobOrderTimeSheet => new {
                JobOrderTimeSheetId = jobOrderTimeSheet.JobOrderTimeSheetId,
                JobOrderTimeRegistrationId = jobOrderTimeSheet.JobOrderTimeRegistrationId,
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
        public ActionResult JobOrderTimeSheets_Create([DataSourceRequest]DataSourceRequest request, JobOrderTimeSheet jobOrderTimeSheet, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeSheet
                {
                    JobOrderTimeRegistrationId = id,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.JobOrderTimeSheets.Add(entity);
                db.SaveChanges();
                jobOrderTimeSheet.JobOrderTimeSheetId = entity.JobOrderTimeSheetId;
            }

            return Json(new[] { jobOrderTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeSheets_Update([DataSourceRequest]DataSourceRequest request, JobOrderTimeSheet jobOrderTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeSheet
                {
                    JobOrderTimeSheetId = jobOrderTimeSheet.JobOrderTimeSheetId,
                    JobOrderTimeRegistrationId = jobOrderTimeSheet.JobOrderTimeRegistrationId,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.JobOrderTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeSheets_Destroy([DataSourceRequest]DataSourceRequest request, JobOrderTimeSheet jobOrderTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeSheet
                {
                    JobOrderTimeSheetId = jobOrderTimeSheet.JobOrderTimeSheetId,
                    JobOrderTimeRegistrationId = jobOrderTimeSheet.JobOrderTimeRegistrationId,
                    Date = jobOrderTimeSheet.Date,
                    Work = jobOrderTimeSheet.Work,
                    StartTime = jobOrderTimeSheet.StartTime,
                    ComplationTime = jobOrderTimeSheet.ComplationTime,
                    TotalTime = jobOrderTimeSheet.TotalTime,
                    NoOfEmployee = jobOrderTimeSheet.NoOfEmployee,
                    Remark = jobOrderTimeSheet.Remark
                };

                db.JobOrderTimeSheets.Attach(entity);
                db.JobOrderTimeSheets.Remove(entity);
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
