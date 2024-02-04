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
    public class JobOrderSummaryCostCentersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult JobOrderSummaryCostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobOrderSummaryCostCenter> jobordersummarycostcenters = db.JobOrderSummaryCostCenters;
            DataSourceResult result = jobordersummarycostcenters.ToDataSourceResult(request, jobOrderSummaryCostCenter => new {
                JobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.JobOrderSummaryCostCenterId,
                JobId = jobOrderSummaryCostCenter.JobId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Create([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummaryCostCenter
                {
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.JobOrderSummaryCostCenters.Add(entity);
                db.SaveChanges();
                jobOrderSummaryCostCenter.JobOrderSummaryCostCenterId = entity.JobOrderSummaryCostCenterId;
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Update([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummaryCostCenter
                {
                    JobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.JobOrderSummaryCostCenterId,
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.JobOrderSummaryCostCenters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobOrderSummaryCostCenters_Destroy([DataSourceRequest]DataSourceRequest request, JobOrderSummaryCostCenter jobOrderSummaryCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobOrderSummaryCostCenter
                {
                    JobOrderSummaryCostCenterId = jobOrderSummaryCostCenter.JobOrderSummaryCostCenterId,
                    JobId = jobOrderSummaryCostCenter.JobId
                };

                db.JobOrderSummaryCostCenters.Attach(entity);
                db.JobOrderSummaryCostCenters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobOrderSummaryCostCenter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
