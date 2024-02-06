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
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation

{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class FurnitureJobStatusLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobStatusLineItems_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<FurnitureJobStatusLineItem> jobstatuslineitems = db.FurnitureJobStatusLineItems.Where(q=>q.FurnitureJobStatusId == id);
            DataSourceResult result = jobstatuslineitems.ToDataSourceResult(request, jobStatusLineItem => new {
                JobStatusLineItemId = jobStatusLineItem.FurnitureJobStatusLineItemId,
                JobStatusId = jobStatusLineItem.FurnitureJobStatusId,
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

        public  ActionResult JobStatusLineItems_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobStatusLineItem jobStatusLineItem, int id)
        {
            var jobStstus = db.FurnitureJobStatuses.Find(id);
            var job = db.FurnitureJobs.Find(jobStstus?.FurnitureJobId);
            var changeOfOrders = db.FurnitureChangeOrders.Where(q => q.FurnitureJobId == jobStstus.FurnitureJobId).ToList();
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
                var entity = (FurnitureJobStatusLineItem)this.GetObject(jobStatusLineItem, true, id);
                db.FurnitureJobStatusLineItems.Add(entity);
               
                    ChangeJobStatus(jobStstus, jobStatusLineItem.RemainigQuantiy, entity.Date);                   
                db.SaveChanges();

                jobStatusLineItem.FurnitureJobStatusId = entity.FurnitureJobStatusId;
            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }

        private void ChangeJobStatus(FurnitureJobStatus jobStstus, decimal? remainigQuantiy, DateTime date)
        {
            if(jobStstus!=null)
            {
              
                jobStstus.EndDate = date;
                if (remainigQuantiy == 0)
                {
                    jobStstus.Status = FurnitureJobState.Finished;

                }
                else
                {
                    jobStstus.Status = FurnitureJobState.ParitiallyFinished;
                }
                db.FurnitureJobStatuses.Attach(jobStstus);
                db.Entry(jobStstus).State = EntityState.Modified;
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatusLineItems_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobStatusLineItem jobStatusLineItem)
        {
            var jobStstus = db.FurnitureJobStatuses.Find(jobStatusLineItem.FurnitureJobStatusId);
            var job = db.Jobs.Find(jobStstus?.FurnitureJobId);
            var jobStatus = db.FurnitureJobStatusLineItems.Where(q => q.FurnitureJobStatusId == jobStatusLineItem.FurnitureJobStatusId
            && q.FurnitureJobStatusLineItemId != jobStatusLineItem.FurnitureJobStatusLineItemId).ToList();
            var totQuan = jobStatus.Sum(s => s.Quantiy) + jobStatusLineItem.Quantiy;
            var orderedQuant = job?.Quantity;
            if (totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }

            if (ModelState.IsValid)
            {
                jobStatusLineItem.RemainigQuantiy = orderedQuant - totQuan;
                var entity = (FurnitureJobStatusLineItem)this.GetObject(jobStatusLineItem, false);

                db.FurnitureJobStatusLineItems.Attach(entity);
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
        public ActionResult JobStatusLineItems_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobStatusLineItem jobStatusLineItem)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobStatusLineItem)this.GetObject(jobStatusLineItem, false);

                db.FurnitureJobStatusLineItems.Attach(entity);
                db.FurnitureJobStatusLineItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobStatusLineItem jobStatusLineItem, bool status, int id = 0)
        {
            if (status)
            {
                jobStatusLineItem.FurnitureJobStatusId = id;
            }
            var entity = new FurnitureJobStatusLineItem
            {
                FurnitureJobStatusLineItemId = jobStatusLineItem.FurnitureJobStatusLineItemId,
                FurnitureJobStatusId = jobStatusLineItem.FurnitureJobStatusId,
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
