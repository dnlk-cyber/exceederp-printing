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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class FurnitureDeliveredJobController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeliveredJobs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureDeliveredJob> deliveredjobs = db.FurnitureDeliveredJobs;
            DataSourceResult result = deliveredjobs.ToDataSourceResult(request, deliveredJob => new {
                DeliveredJobId = deliveredJob.FurnitureDeliveredJobId,
                JobCostId = deliveredJob.FurnitureJobCostId,
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

        public ActionResult DeliveredJobs_Create([DataSourceRequest]DataSourceRequest request, FurnitureDeliveredJob deliveredJob, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureDeliveredJob)this.GetObject(deliveredJob, true, id);
                db.FurnitureDeliveredJobs.Add(entity);
                db.SaveChanges();
                deliveredJob.FurnitureDeliveredJobId = entity.FurnitureDeliveredJobId;
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeliveredJobs_Update([DataSourceRequest]DataSourceRequest request, FurnitureDeliveredJob deliveredJob)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureDeliveredJob)this.GetObject(deliveredJob, false);

                db.FurnitureDeliveredJobs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeliveredJobs_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureDeliveredJob deliveredJob)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureDeliveredJob)this.GetObject(deliveredJob, false);

                db.FurnitureDeliveredJobs.Attach(entity);
                db.FurnitureDeliveredJobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { deliveredJob }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureDeliveredJob deliveredJob, bool status, int id = 0)
        {
            if (status)
            {
                deliveredJob.FurnitureJobCostId = id;
            }
            var entity = new FurnitureDeliveredJob
            {
                FurnitureDeliveredJobId = deliveredJob.FurnitureDeliveredJobId,
                FurnitureJobCostId = deliveredJob.FurnitureJobCostId,
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