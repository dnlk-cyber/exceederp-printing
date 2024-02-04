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
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class DeliveredJobController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeliveredJobs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DeliveredJob> deliveredjobs = db.DeliveredJobs;
            DataSourceResult result = deliveredjobs.ToDataSourceResult(request, deliveredJob => new {
                DeliveredJobId = deliveredJob.DeliveredJobId,
                JobCostId = deliveredJob.JobCostId,
                DoNo = deliveredJob.DoNo,
                Date = deliveredJob.Date,
                Quantity = deliveredJob.Quantity,
                DateCreated = deliveredJob.DateCreated,
                LastModified = deliveredJob.LastModified,
                CreatedBy = deliveredJob.CreatedBy,
                ModifiedBy = deliveredJob.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult DeliveredJobs_Create([DataSourceRequest]DataSourceRequest request, DeliveredJob deliveredJob, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (DeliveredJob)this.GetObject(deliveredJob, true, id);
                db.DeliveredJobs.Add(entity);
                db.SaveChanges();
                deliveredJob.DeliveredJobId = entity.DeliveredJobId;
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeliveredJobs_Update([DataSourceRequest]DataSourceRequest request, DeliveredJob deliveredJob)
        {
            if (ModelState.IsValid)
            {
                var entity = (DeliveredJob)this.GetObject(deliveredJob, false);

                db.DeliveredJobs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeliveredJobs_Destroy([DataSourceRequest]DataSourceRequest request, DeliveredJob deliveredJob)
        {

            if (ModelState.IsValid)
            {
                var entity = (DeliveredJob)this.GetObject(deliveredJob, false);

                db.DeliveredJobs.Attach(entity);
                db.DeliveredJobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(DeliveredJob deliveredJob, bool status, int id = 0)
        {
            if (status)
            {
                deliveredJob.JobCostId = id;
            }
            var entity = new DeliveredJob
            {
                DeliveredJobId = deliveredJob.DeliveredJobId,
                JobCostId = deliveredJob.JobCostId,
                DoNo = deliveredJob.DoNo,
                Date = deliveredJob.Date,
                Quantity = deliveredJob.Quantity,
                DateCreated = deliveredJob.DateCreated,
                LastModified = deliveredJob.LastModified,
                CreatedBy = deliveredJob.CreatedBy,
                ModifiedBy = deliveredJob.ModifiedBy
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