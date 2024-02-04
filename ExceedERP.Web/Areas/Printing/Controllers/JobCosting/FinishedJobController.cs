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
    public class FinishedJobController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinishedJobs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FinishedJob> finishedjobs = db.FinishedJobs.Where(q=>q.JobCostId == id);
            DataSourceResult result = finishedjobs.ToDataSourceResult(request, finishedJob => new {
                FinishedJobId = finishedJob.FinishedJobId,
                JobCostId = finishedJob.JobCostId,
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

        public ActionResult FinishedJobs_Create([DataSourceRequest]DataSourceRequest request, FinishedJob finishedJob, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FinishedJob)this.GetObject(finishedJob, true, id);
                db.FinishedJobs.Add(entity);
                db.SaveChanges();
                finishedJob.FinishedJobId = entity.FinishedJobId;
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedJobs_Update([DataSourceRequest]DataSourceRequest request, FinishedJob finishedJob)
        {
            if (ModelState.IsValid)
            {
                var entity = (FinishedJob)this.GetObject(finishedJob, false);

                db.FinishedJobs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedJobs_Destroy([DataSourceRequest]DataSourceRequest request, FinishedJob finishedJob)
        {

            if (ModelState.IsValid)
            {
                var entity = (FinishedJob)this.GetObject(finishedJob, false);

                db.FinishedJobs.Attach(entity);
                db.FinishedJobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { finishedJob }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FinishedJob finishedJob, bool status, int id = 0)
        {
            if (status)
            {
                finishedJob.JobCostId = id;
            }
            var entity = new FinishedJob
            {
                FinishedJobId = finishedJob.FinishedJobId,
                JobCostId = finishedJob.JobCostId,
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