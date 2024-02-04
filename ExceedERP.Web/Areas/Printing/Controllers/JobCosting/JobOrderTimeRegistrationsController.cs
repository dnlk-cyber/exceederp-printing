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
    public class JobOrderTimeRegistrationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult JobOrderTimeRegistrations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobOrderTimeRegistration> jobordertimeregistrations = db.JobOrderTimeRegistrations;
            DataSourceResult result = jobordertimeregistrations.ToDataSourceResult(request, jobOrderTimeRegistration => new {
                JobOrderTimeRegistrationId = jobOrderTimeRegistration.JobOrderTimeRegistrationId,
                JobId = jobOrderTimeRegistration.JobId,
                Remark = jobOrderTimeRegistration.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Create([DataSourceRequest]DataSourceRequest request, JobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeRegistration
                {
                    JobId = jobOrderTimeRegistration.JobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.JobOrderTimeRegistrations.Add(entity);
                db.SaveChanges();
                jobOrderTimeRegistration.JobOrderTimeRegistrationId = entity.JobOrderTimeRegistrationId;
            }

            return Json(new[] { jobOrderTimeRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Update([DataSourceRequest]DataSourceRequest request, JobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeRegistration
                {
                    JobOrderTimeRegistrationId = jobOrderTimeRegistration.JobOrderTimeRegistrationId,
                    JobId = jobOrderTimeRegistration.JobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.JobOrderTimeRegistrations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Destroy([DataSourceRequest]DataSourceRequest request, JobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderTimeRegistration
                {
                    JobOrderTimeRegistrationId = jobOrderTimeRegistration.JobOrderTimeRegistrationId,
                    JobId = jobOrderTimeRegistration.JobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.JobOrderTimeRegistrations.Attach(entity);
                db.JobOrderTimeRegistrations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeRegistration }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
