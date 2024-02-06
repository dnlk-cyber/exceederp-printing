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
    public class FurnitureJobOrderTimeRegistrationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult JobOrderTimeRegistrations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderTimeRegistration> jobordertimeregistrations = db.FurnitureJobOrderTimeRegistrations;
            DataSourceResult result = jobordertimeregistrations.ToDataSourceResult(request, jobOrderTimeRegistration => new {
                JobOrderTimeRegistrationId = jobOrderTimeRegistration.FurnitureJobOrderTimeRegistrationId,
                JobId = jobOrderTimeRegistration.FurnitureJobId,
                Remark = jobOrderTimeRegistration.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeRegistration
                {
                    FurnitureJobId = jobOrderTimeRegistration.FurnitureJobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.FurnitureJobOrderTimeRegistrations.Add(entity);
                db.SaveChanges();
                jobOrderTimeRegistration.FurnitureJobOrderTimeRegistrationId = entity.FurnitureJobOrderTimeRegistrationId;
            }

            return Json(new[] { jobOrderTimeRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeRegistration
                {
                    FurnitureJobOrderTimeRegistrationId = jobOrderTimeRegistration.FurnitureJobOrderTimeRegistrationId,
                    FurnitureJobId = jobOrderTimeRegistration.FurnitureJobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.FurnitureJobOrderTimeRegistrations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderTimeRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderTimeRegistrations_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderTimeRegistration jobOrderTimeRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderTimeRegistration
                {
                    FurnitureJobOrderTimeRegistrationId = jobOrderTimeRegistration.FurnitureJobOrderTimeRegistrationId,
                    FurnitureJobId = jobOrderTimeRegistration.FurnitureJobId,
                    Remark = jobOrderTimeRegistration.Remark
                };

                db.FurnitureJobOrderTimeRegistrations.Attach(entity);
                db.FurnitureJobOrderTimeRegistrations.Remove(entity);
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
