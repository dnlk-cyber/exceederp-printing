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
    public class ProductDeliveryItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductDeliveryItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductDeliveryItems> productdeliveryitems = db.ProductDeliveryItems.Where(x=>x.ProductDeliveryId == id);
            DataSourceResult result = productdeliveryitems.ToDataSourceResult(request, productDeliveryItems => new {
                ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                ItemCode = productDeliveryItems.ItemCode,
                Description = productDeliveryItems.Description,
                UnitId = productDeliveryItems.UnitId,
                Quantity = productDeliveryItems.Quantity,
                UnitCost = productDeliveryItems.UnitCost,
                TotalCost = productDeliveryItems.TotalCost
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Create([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryId = id,
                    ItemCode = productDeliveryItems.ItemCode,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.ProductDeliveryItems.Add(entity);
                db.SaveChanges();
                productDeliveryItems.ProductDeliveryItemsId = entity.ProductDeliveryItemsId;
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Update([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                    ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                    ItemCode = productDeliveryItems.ItemCode,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.ProductDeliveryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Destroy([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                    ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                    ItemCode = productDeliveryItems.ItemCode,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.ProductDeliveryItems.Attach(entity);
                db.ProductDeliveryItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
