﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Setting
{
    public class JobTypesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["TaxRate"] = db.GLTaxRates.ToList();
            return View();
        }

        public ActionResult JobTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobType> jobtypes = db.JobTypes;
            DataSourceResult result = jobtypes.ToDataSourceResult(request, jobType => new {
                JobTypeId = jobType.JobTypeId,
                Type = jobType.Type,
                GLTaxRateID = jobType.GLTaxRateID
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypes_Create([DataSourceRequest]DataSourceRequest request, JobType jobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobType
                {
                    Type = jobType.Type,
                    GLTaxRateID = jobType.GLTaxRateID
                };

                db.JobTypes.Add(entity);
                db.SaveChanges();
                jobType.JobTypeId = entity.JobTypeId;
            }

            return Json(new[] { jobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypes_Update([DataSourceRequest]DataSourceRequest request, JobType jobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobType
                {
                    JobTypeId = jobType.JobTypeId,
                    Type = jobType.Type,
                    GLTaxRateID = jobType.GLTaxRateID
                };

                db.JobTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypes_Destroy([DataSourceRequest]DataSourceRequest request, JobType jobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobType
                {
                    JobTypeId = jobType.JobTypeId,
                    Type = jobType.Type,
                    GLTaxRateID = jobType.GLTaxRateID
                };

                db.JobTypes.Attach(entity);
                db.JobTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
