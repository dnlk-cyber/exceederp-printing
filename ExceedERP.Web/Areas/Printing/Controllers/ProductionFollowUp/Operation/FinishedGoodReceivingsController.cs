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

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FinishedGoodReceivingsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.OrganizationCustomers.Select(s=>new {
            Code = s.OrganizationCustomerID,
            Name = s.TradeName
            }).ToList();

            ViewData["JobOrder"] = db.Jobs.ToList();
            return View();
        }

        public ActionResult FinishedGoodReceivings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingProductionFinishedGoodsStoreReceive> finishedgoodreceivings = db.PrintingProductionFinishedGoodsStoreReceives;
            DataSourceResult result = finishedgoodreceivings.ToDataSourceResult(request, finishedGoodReceiving => new {
                FinishedGoodReceivingId = finishedGoodReceiving.FinishedGoodsStoreReceiveId,
                VoucherNo = finishedGoodReceiving.VoucherNo,
                CustomerId = finishedGoodReceiving.CustomerId,
                StoreCode = finishedGoodReceiving.StoreCode,
                JobId = finishedGoodReceiving.JobId,
                DeliveredBy = finishedGoodReceiving.DeliveredBy,
                ReceivedBy = finishedGoodReceiving.ReceivedBy,
                Supervisor = finishedGoodReceiving.Supervisor
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Create([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.Jobs.Find(finishedGoodReceiving.JobId);

                var entity = new PrintingProductionFinishedGoodsStoreReceive
                {
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    CustomerId = jobOrder.CustomerId,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.PrintingProductionFinishedGoodsStoreReceives.Add(entity);
                db.SaveChanges();
                finishedGoodReceiving.FinishedGoodsStoreReceiveId = entity.FinishedGoodsStoreReceiveId;
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Update([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.Jobs.Find(finishedGoodReceiving.JobId);
                var entity = new PrintingProductionFinishedGoodsStoreReceive
                {
                    FinishedGoodsStoreReceiveId = finishedGoodReceiving.FinishedGoodsStoreReceiveId,
                    CustomerId = jobOrder.CustomerId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.PrintingProductionFinishedGoodsStoreReceives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFinishedGoodsStoreReceive
                {
                    FinishedGoodsStoreReceiveId = finishedGoodReceiving.FinishedGoodsStoreReceiveId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    CustomerId = finishedGoodReceiving.CustomerId,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    JobId = finishedGoodReceiving.JobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.PrintingProductionFinishedGoodsStoreReceives.Attach(entity);
                db.PrintingProductionFinishedGoodsStoreReceives.Remove(entity);
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
