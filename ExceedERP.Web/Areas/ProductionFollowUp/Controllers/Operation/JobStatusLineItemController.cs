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

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
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
                TotalPrice = jobStatusLineItem.TotalPrice,
                Invoice = jobStatusLineItem.Invoice,
                SalesVoucherNo = jobStatusLineItem.SalesVoucherNo,
                Cash = jobStatusLineItem.Cash,
                Credit = jobStatusLineItem.Credit,
                CashCreditInvoice = jobStatusLineItem.CashCreditInvoice
            });

            return Json(result);
        }

        public ActionResult JobStatusLineItems_Create([DataSourceRequest]DataSourceRequest request, JobStatusLineItem jobStatusLineItem, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobStatusLineItem)this.GetObject(jobStatusLineItem, true, id);
                db.JobStatusLineItems.Add(entity);
                db.SaveChanges();
                jobStatusLineItem.JobStatusId = entity.JobStatusId;
            }

            return Json(new[] { jobStatusLineItem }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobStatusLineItems_Update([DataSourceRequest]DataSourceRequest request, JobStatusLineItem jobStatusLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobStatusLineItem)this.GetObject(jobStatusLineItem, false);

                db.JobStatusLineItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
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
