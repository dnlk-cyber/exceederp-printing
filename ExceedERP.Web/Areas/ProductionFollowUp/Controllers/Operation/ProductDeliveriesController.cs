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
    public class ProductDeliveriesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.Customers.ToList();
            ViewData["JobOrder"] = db.Jobs.ToList();
            ViewData["Units"] = db.Units.ToList();
            return View();
        }

        public ActionResult ProductDeliveries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductDelivery> productdeliveries = db.ProductDeliveries;
            DataSourceResult result = productdeliveries.ToDataSourceResult(request, productDelivery => new {
                ProductDeliveryId = productDelivery.ProductDeliveryId,
                CustomerId = productDelivery.CustomerId,
                CustomerRefNo = productDelivery.CustomerRefNo,
                CreditSaleNo = productDelivery.CreditSaleNo,
                CashSaleNo = productDelivery.CashSaleNo,
                PreparedBy = productDelivery.PreparedBy,
                ApprovedBy = productDelivery.ApprovedBy,
                IdentificationNo = productDelivery.IdentificationNo,
                JobId = productDelivery.JobId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Create([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var finishedGood = db.Jobs.Find(productDelivery.JobId);
                var entity = new ProductDelivery
                {
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CustomerId = finishedGood.CustomerId,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Add(entity);
                db.SaveChanges();
                productDelivery.ProductDeliveryId = entity.ProductDeliveryId;
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Update([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var finishedGood = db.Jobs.Find(productDelivery.JobId);

                var entity = new ProductDelivery
                {
                    ProductDeliveryId = productDelivery.ProductDeliveryId,
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CustomerId = finishedGood.CustomerId,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Destroy([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDelivery
                {
                    ProductDeliveryId = productDelivery.ProductDeliveryId,
                    CustomerId = productDelivery.CustomerId,
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Attach(entity);
                db.ProductDeliveries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
