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
    public class FurnitureFinishedJobController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinishedJobs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureFinishedJob> finishedjobs = db.FurnitureFinishedJobs.Where(q=>q.FurnitureJobCostId == id);
            DataSourceResult result = finishedjobs.ToDataSourceResult(request, finishedJob => new {
                FurnitureFinishedJobId = finishedJob.FurnitureFinishedJobId,
                FurnitureJobCostId = finishedJob.FurnitureJobCostId,
                FgrNo = finishedJob.FgrNo,
                Date = finishedJob.Date,
                Quantity = finishedJob.Quantity,
                DateCreated = finishedJob.DateCreated,
                LastModified = finishedJob.LastModified,
                CreatedBy = finishedJob.CreatedBy,
                ModifiedBy = finishedJob.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult FinishedJobs_Create([DataSourceRequest]DataSourceRequest request, FurnitureFinishedJob finishedJob, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureFinishedJob)this.GetObject(finishedJob, true, id);
                db.FurnitureFinishedJobs.Add(entity);
                db.SaveChanges();
                finishedJob.FurnitureFinishedJobId = entity.FurnitureFinishedJobId;
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedJobs_Update([DataSourceRequest]DataSourceRequest request, FurnitureFinishedJob finishedJob)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureFinishedJob)this.GetObject(finishedJob, false);

                db.FurnitureFinishedJobs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedJobs_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureFinishedJob finishedJob)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureFinishedJob)this.GetObject(finishedJob, false);

                db.FurnitureFinishedJobs.Attach(entity);
                db.FurnitureFinishedJobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureFinishedJob finishedJob, bool status, int id = 0)
        {
            if (status)
            {
                finishedJob.FurnitureJobCostId = id;
            }
            var entity = new FurnitureFinishedJob
            {
                FurnitureFinishedJobId = finishedJob.FurnitureFinishedJobId,
                FurnitureJobCostId = finishedJob.FurnitureJobCostId,
                FgrNo = finishedJob.FgrNo,
                Date = finishedJob.Date,
                Quantity = finishedJob.Quantity,
                DateCreated = finishedJob.DateCreated,
                LastModified = finishedJob.LastModified,
                CreatedBy = finishedJob.CreatedBy,
                ModifiedBy = finishedJob.ModifiedBy
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