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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class JobStatusLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobStatusLineItems_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<JobStatusLineItem> jobstatuslineitems = db.JobStatusLineItems.Where(q=>q.JobStatusId==id);
            DataSourceResult result = jobstatuslineitems.ToDataSourceResult(request, jobStatusLineItem => new {
                JobStatusLineItemId = jobStatusLineItem.JobStatusLineItemId,
                JobStatusId = jobStatusLineItem.JobStatusId,
                Date = jobStatusLineItem.Date,
                Quantiy = jobStatusLineItem.Quantiy,
                RemainigQuantiy = jobStatusLineItem.RemainigQuantiy,
                TotalPrice = jobStatusLineItem.TotalPrice,
                Invoice = jobStatusLineItem.Invoice,
                SalesVoucherNo = jobStatusLineItem.SalesVoucherNo,
                Cash = jobStatusLineItem.Cash,
                Credit = jobStatusLineItem.Credit,
                CashCreditInvoice = jobStatusLineItem.CashCreditInvoice
            });

            return Json(result);
        }

        public  ActionResult JobStatusLineItems_Create([DataSourceRequest]DataSourceRequest request, JobStatusLineItem jobStatusLineItem, int id)
        {
            var jobStstus = db.JobStatuses.Find(id);
            var job = db.Jobs.Find(jobStstus?.JobId);
            var changeOfOrders = db.ChangeOrders.Where(q => q.JobId == jobStstus.JobId).ToList();
            var jobStatus = db.JobStatusLineItems.Where(q=>q.JobStatusId== id).ToList();
            
            var totQuan = jobStatus.Sum(s => s.Quantiy) + jobStatusLineItem.Quantiy;
            var orderedQuant = job?.Quantity + changeOfOrders.Sum(s=>s.Quantity);
            if (totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error","The items sum must be equals to the ordered quantity");
            }

            if (ModelState.IsValid)
            {
                jobStatusLineItem.RemainigQuantiy = orderedQuant - totQuan;
                var entity = (JobStatusLineItem)this.GetObject(jobStatusLineItem, true, id);
                db.JobStatusLineItems.Add(entity);
               
                    ChangeJobStatus(jobStstus,jobStatusLineItem.RemainigQuantiy, entity.Date);                   
                db.SaveChanges();

                jobStatusLineItem.JobStatusId = entity.JobStatusId;
            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }

        private void ChangeJobStatus(JobStatus jobStstus, decimal? remainigQuantiy, DateTime date)
        {
            if(jobStstus!=null)
            {
              
                jobStstus.EndDate = date;
                if (remainigQuantiy == 0)
                {
                    jobStstus.Status = JobState.Finished;

                }
                else
                {
                    jobStstus.Status = JobState.ParitiallyFinished;
                }
                db.JobStatuses.Attach(jobStstus);
                db.Entry(jobStstus).State = EntityState.Modified;
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatusLineItems_Update([DataSourceRequest]DataSourceRequest request, JobStatusLineItem jobStatusLineItem)
        {
            var jobStstus = db.JobStatuses.Find(jobStatusLineItem.JobStatusId);
            var job = db.Jobs.Find(jobStstus?.JobId);
            var jobStatus = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatusLineItem.JobStatusId
            && q.JobStatusLineItemId != jobStatusLineItem.JobStatusLineItemId).ToList();
            var totQuan = jobStatus.Sum(s => s.Quantiy) + jobStatusLineItem.Quantiy;
            var orderedQuant = job?.Quantity;
            if (totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }

            if (ModelState.IsValid)
            {
                jobStatusLineItem.RemainigQuantiy = orderedQuant - totQuan;
                var entity = (JobStatusLineItem)this.GetObject(jobStatusLineItem, false);

                db.JobStatusLineItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                if (jobStatusLineItem.RemainigQuantiy == 0)
                {
                    ChangeJobStatus(jobStstus, jobStatusLineItem.RemainigQuantiy, entity.Date);
                }
                db.SaveChanges();

            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatusLineItems_Destroy([DataSourceRequest]DataSourceRequest request, JobStatusLineItem jobStatusLineItem)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobStatusLineItem)this.GetObject(jobStatusLineItem, false);

                db.JobStatusLineItems.Attach(entity);
                db.JobStatusLineItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobStatusLineItem jobStatusLineItem, bool status, int id = 0)
        {
            if (status)
            {
                jobStatusLineItem.JobStatusId = id;
            }
            var entity = new JobStatusLineItem
            {
                JobStatusLineItemId = jobStatusLineItem.JobStatusLineItemId,
                JobStatusId = jobStatusLineItem.JobStatusId,
                Date = jobStatusLineItem.Date,
                Quantiy = jobStatusLineItem.Quantiy,
                RemainigQuantiy = jobStatusLineItem.RemainigQuantiy,
                TotalPrice = jobStatusLineItem.TotalPrice,
                Invoice = jobStatusLineItem.Invoice,
                SalesVoucherNo = jobStatusLineItem.SalesVoucherNo,
                Cash = jobStatusLineItem.Cash,
                Credit = jobStatusLineItem.Credit,
                CashCreditInvoice = jobStatusLineItem.CashCreditInvoice

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
