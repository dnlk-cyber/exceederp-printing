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

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation

{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FurnitureFinishedGoodReceivingsController : Controller
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
            IQueryable<FurnitureProductionFinishedGoodsStoreReceive> finishedgoodreceivings = db.FurnitureProductionFinishedGoodsStoreReceives;
            DataSourceResult result = finishedgoodreceivings.ToDataSourceResult(request, finishedGoodReceiving => new {
                FinishedGoodReceivingId = finishedGoodReceiving.FurnitureFinishedGoodsStoreReceiveId,
                VoucherNo = finishedGoodReceiving.VoucherNo,
                CustomerId = finishedGoodReceiving.FurnitureCustomerId,
                StoreCode = finishedGoodReceiving.StoreCode,
                JobId = finishedGoodReceiving.FurnitureJobId,
                DeliveredBy = finishedGoodReceiving.DeliveredBy,
                ReceivedBy = finishedGoodReceiving.ReceivedBy,
                Supervisor = finishedGoodReceiving.Supervisor
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.FurnitureJobs.Find(finishedGoodReceiving.FurnitureJobId);

                var entity = new FurnitureProductionFinishedGoodsStoreReceive
                {
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    FurnitureCustomerId = jobOrder.FurnitureCustomerId,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    FurnitureJobId = finishedGoodReceiving.FurnitureJobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FurnitureProductionFinishedGoodsStoreReceives.Add(entity);
                db.SaveChanges();
                finishedGoodReceiving.FurnitureFinishedGoodsStoreReceiveId = entity.FurnitureFinishedGoodsStoreReceiveId;
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = db.Jobs.Find(finishedGoodReceiving.FurnitureJobId);
                var entity = new FurnitureProductionFinishedGoodsStoreReceive
                {
                    FurnitureFinishedGoodsStoreReceiveId = finishedGoodReceiving.FurnitureFinishedGoodsStoreReceiveId,
                    FurnitureCustomerId = jobOrder.CustomerId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    FurnitureJobId = finishedGoodReceiving.FurnitureJobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FurnitureProductionFinishedGoodsStoreReceives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedGoodReceiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoodReceivings_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceive finishedGoodReceiving)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionFinishedGoodsStoreReceive
                {
                    FurnitureFinishedGoodsStoreReceiveId = finishedGoodReceiving.FurnitureFinishedGoodsStoreReceiveId,
                    VoucherNo = finishedGoodReceiving.VoucherNo,
                    FurnitureCustomerId = finishedGoodReceiving.FurnitureCustomerId,
                    StoreCode = finishedGoodReceiving.StoreCode,
                    FurnitureJobId = finishedGoodReceiving.FurnitureJobId,
                    DeliveredBy = finishedGoodReceiving.DeliveredBy,
                    ReceivedBy = finishedGoodReceiving.ReceivedBy,
                    Supervisor = finishedGoodReceiving.Supervisor
                };

                db.FurnitureProductionFinishedGoodsStoreReceives.Attach(entity);
                db.FurnitureProductionFinishedGoodsStoreReceives.Remove(entity);
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
