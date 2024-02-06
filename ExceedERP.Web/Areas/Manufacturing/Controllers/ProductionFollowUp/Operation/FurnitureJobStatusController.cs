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
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation

{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class FurnitureJobStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChangeOrderStatus()
        {
            var jobStatuses = db.FurnitureJobStatuses;
            foreach(var jobStatus  in jobStatuses)
            {
                if(jobStatus.Status.ToString()== FurnitureJobState.ParitiallyFinished.ToString())
                {
                    jobStatus.Status = FurnitureJobState.Received;
                    var jobLine = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.FurnitureJobStatusId).ToList();
                    if(jobLine.Any())
                    {
                        jobStatus.Status = FurnitureJobState.ParitiallyFinished;
                        var jobOrder = db.FurnitureJobs.Find(jobStatus.FurnitureJobId);
                        if (jobOrder != null)
                        {
                            var changeOrQuan = db.FurnitureChangeOrders.Where(q => q.FurnitureJobId == jobOrder.FurnitureJobId).ToList().Sum(q => q.Quantity);
                            var actual = jobLine.Sum(s => s.Quantiy);
                            var Ordered = changeOrQuan + jobOrder.Quantity;
                            if (actual == Ordered)
                            {
                                jobStatus.Status = FurnitureJobState.Finished;

                            }
                        }
                    }
                   
                    db.FurnitureJobStatuses.Attach(jobStatus);
                    db.Entry(jobStatus).State = EntityState.Modified;
                }
                if(jobStatus.JobPhase == FurnitureProductionDepartment.Graphic)
                {
                    if (jobStatus.Status == FurnitureJobState.ParitiallyFinished)
                    {
                        jobStatus.Status = FurnitureJobState.Finished;
                    }

                }
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult JobStatuses_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobStatus> jobstatuses = db.FurnitureJobStatuses.Where(q=>q.FurnitureJobId == id);
            DataSourceResult result = jobstatuses.ToList().ToDataSourceResult(request, jobStatus => new {
                JobStatusId = jobStatus.FurnitureJobStatusId,
                JobId = jobStatus.FurnitureJobId,
                UserId = jobStatus.FurnitureUserId,
                Time = jobStatus.Time,
                StartDate = jobStatus.StartDate,
                EndDate = jobStatus.EndDate,
                JobPhase = jobStatus.JobPhase,
                Status = jobStatus.Status
                });

            return Json(result);
        }

        public ActionResult JobStatuses_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobStatus jobStatus, int id)
        {
            var jobS = db.FurnitureJobStatuses.Where(q => q.JobPhase == jobStatus.JobPhase
            && q.Status == jobStatus.Status
            &&q.FurnitureJobId == id).FirstOrDefault();
            jobStatus.FurnitureUserId = User.Identity.Name;

            if (jobS!=null)
            {
                ModelState.AddModelError("Error","Existed Phase");
            }
            if (ModelState.IsValid)
            {
                jobStatus.EndDate = jobStatus.StartDate;
                jobStatus.Status = FurnitureJobState.Received;
                   var entity = (FurnitureJobStatus)this.GetObject(jobStatus, true, id);
                db.FurnitureJobStatuses.Add(entity);
                db.SaveChanges();
                jobStatus.FurnitureJobStatusId = entity.FurnitureJobStatusId;
            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatuses_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobStatus jobStatus)
        {
            var jobS = db.FurnitureJobStatuses.Where(q => q.JobPhase == jobStatus.JobPhase 
            && q.Status == jobStatus.Status
            && q.FurnitureJobStatusId != jobStatus.FurnitureJobStatusId
            && q.FurnitureJobId == jobStatus.FurnitureJobId).FirstOrDefault();
            if (jobS != null)
            {
                ModelState.AddModelError("Error", "Existed Phase");
            }
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobStatus)this.GetObject(jobStatus, false);

                if (entity.JobPhase == FurnitureProductionDepartment.Graphic)
                {
                    jobStatus.EndDate = DateTime.Now;
                }
                db.FurnitureJobStatuses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                
            
                db.SaveChanges();

            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatuses_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobStatus jobStatus)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobStatus)this.GetObject(jobStatus, false);

                db.FurnitureJobStatuses.Attach(entity);
                db.FurnitureJobStatuses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobStatus }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobStatus jobStatus, bool status, int id = 0)
        {
            if (status)
            {
                jobStatus.FurnitureJobId = id;
            }
            var entity = new FurnitureJobStatus
            {
                FurnitureJobStatusId = jobStatus.FurnitureJobStatusId,
                FurnitureJobId = jobStatus.FurnitureJobId,
                FurnitureUserId = jobStatus.FurnitureUserId,
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