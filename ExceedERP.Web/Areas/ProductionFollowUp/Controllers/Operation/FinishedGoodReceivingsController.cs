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
    public class FinishedGoodReceivingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.Customers.ToList();

            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult FinishedGoodReceivings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinishedGoodReceiving> finishedgoodreceivings = db.FinishedGoodReceivings;
            DataSourceResult result = finishedgoodreceivings.ToDataSourceResult(request, finishedGoodReceiving => new {
                FinishedGoodReceivingId = finishedGoodReceiving.FinishedGoodReceivingId,
                VoucherNo = finishedGoodReceiving.VoucherNo,
                CustomerId = finishedGoodReceiving.CustomerId,
                Store = finishedGoodReceiving.Store,
                JobId = finishedGoodReceiving.JobId,
                DeliveredBy = finishedGoodReceiving.DeliveredBy,
                ReceivedBy = finishedGoodReceiving.ReceivedBy,
                Supervisor = finishedGoodReceiving.Supervisor
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Create([DataSourceRequest]DataSourceRequest request, FinishedGoodReceiving finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.Jobs.Find(finishedGoodReceiving.JobId);

                var entity = new FinishedGoodReceiving
                {
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    CustomerId = jobOrder.CustomerId,
                    Store = finishedGoodReceiving.Store,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FinishedGoodReceivings.Add(entity);
                db.SaveChanges();
                finishedGoodReceiving.FinishedGoodReceivingId = entity.FinishedGoodReceivingId;
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Update([DataSourceRequest]DataSourceRequest request, FinishedGoodReceiving finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.Jobs.Find(finishedGoodReceiving.JobId);
                var entity = new FinishedGoodReceiving
                {
                    FinishedGoodReceivingId = finishedGoodReceiving.FinishedGoodReceivingId,
                    CustomerId = jobOrder.CustomerId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    Store = finishedGoodReceiving.Store,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FinishedGoodReceivings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Destroy([DataSourceRequest]DataSourceRequest request, FinishedGoodReceiving finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinishedGoodReceiving
                {
                    FinishedGoodReceivingId = finishedGoodReceiving.FinishedGoodReceivingId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    CustomerId = finishedGoodReceiving.CustomerId,
                    Store = finishedGoodReceiving.Store,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FinishedGoodReceivings.Attach(entity);
                db.FinishedGoodReceivings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
