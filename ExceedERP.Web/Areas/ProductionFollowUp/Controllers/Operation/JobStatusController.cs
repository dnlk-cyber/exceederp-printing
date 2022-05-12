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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.PrintingEstimation.Setting;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class JobStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobStatuses_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobStatus> jobstatuses = db.JobStatuses.Where(q=>q.JobId==id);
            DataSourceResult result = jobstatuses.ToList().ToDataSourceResult(request, jobStatus => new {
                JobStatusId = jobStatus.JobStatusId,
                JobId = jobStatus.JobId,
                UserId = jobStatus.UserId,
                Time = jobStatus.Time,
                StartDate = jobStatus.StartDate,
                EndDate = jobStatus.EndDate,
                JobPhase = jobStatus.JobPhase,

                Status = jobStatus.JobPhase == ProductionDepartment.Graphic?jobStatus.Status:(db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(s => s.Quantiy) ==
               (db.Jobs.Where(q => q.JobId == jobStatus.JobId).ToList().Sum(s => s.Quantity) + db.ChangeOrders.Where(q => q.JobId == jobStatus.JobId).ToList().Sum(s => s.Quantity)) ? JobState.Finished : JobState.InProgress)
            });

            return Json(result);
        }

        public ActionResult JobStatuses_Create([DataSourceRequest]DataSourceRequest request, JobStatus jobStatus, int id)
        {
            var jobS = db.JobStatuses.Where(q => q.JobPhase == jobStatus.JobPhase
            && q.Status == jobStatus.Status
            &&q.JobId==id).FirstOrDefault();
            jobStatus.UserId = User.Identity.Name;

            if (jobS!=null)
            {
                ModelState.AddModelError("Error","Existed Phase");
            }
            if (ModelState.IsValid)
            {
                jobStatus.EndDate = jobStatus.StartDate;
                   var entity = (JobStatus)this.GetObject(jobStatus, true, id);
                db.JobStatuses.Add(entity);
                db.SaveChanges();
                jobStatus.JobStatusId = entity.JobStatusId;
            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatuses_Update([DataSourceRequest]DataSourceRequest request, JobStatus jobStatus)
        {
            var jobS = db.JobStatuses.Where(q => q.JobPhase == jobStatus.JobPhase 
            && q.Status == jobStatus.Status
            && q.JobStatusId!= jobStatus.JobStatusId
            && q.JobId == jobStatus.JobId).FirstOrDefault();
            if (jobS != null)
            {
                ModelState.AddModelError("Error", "Existed Phase");
            }
            if (ModelState.IsValid)
            {
                var entity = (JobStatus)this.GetObject(jobStatus, false);

                if (entity.JobPhase == ProductionDepartment.Graphic)
                {
                    jobStatus.EndDate = DateTime.Now;
                }
                db.JobStatuses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                
            
                db.SaveChanges();

            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatuses_Destroy([DataSourceRequest]DataSourceRequest request, JobStatus jobStatus)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobStatus)this.GetObject(jobStatus, false);

                db.JobStatuses.Attach(entity);
                db.JobStatuses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobStatus jobStatus, bool status, int id = 0)
        {
            if (status)
            {
                jobStatus.JobId = id;
            }
            var entity = new JobStatus
            {
                JobStatusId = jobStatus.JobStatusId,
                JobId = jobStatus.JobId,
                UserId = jobStatus.UserId,
                Time = jobStatus.Time,
                StartDate = jobStatus.StartDate,
                EndDate = jobStatus.EndDate,
                JobPhase = jobStatus.JobPhase,
                Status = jobStatus.Status

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