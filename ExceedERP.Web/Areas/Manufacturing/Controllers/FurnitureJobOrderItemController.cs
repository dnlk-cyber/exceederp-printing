﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureJobOrderItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureJobOrderItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobOrderItem> furniturejoborderitems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == id);
            DataSourceResult result = furniturejoborderitems.ToDataSourceResult(request, furnitureJobOrderItem => new FurnitureJobOrderItem
            {
                FurnitureJobOrderItemId = furnitureJobOrderItem.FurnitureJobOrderItemId,
                JobTypeId = furnitureJobOrderItem.JobTypeId,
                UnitOfMeasurment = furnitureJobOrderItem.UnitOfMeasurment,
                Quantity = furnitureJobOrderItem.Quantity,
                UnitPriceBeforeVAT = furnitureJobOrderItem.UnitPriceBeforeVAT,
                VAT = furnitureJobOrderItem.VAT,
                UnitPriceWithVAT = furnitureJobOrderItem.UnitPriceWithVAT,
                DateCreated = furnitureJobOrderItem.DateCreated,
                LastModified = furnitureJobOrderItem.LastModified,
                CreatedBy = furnitureJobOrderItem.CreatedBy,
                ModifiedBy = furnitureJobOrderItem.ModifiedBy,
                FurnitureJobOrderFormId = furnitureJobOrderItem.FurnitureJobOrderFormId,
                JobNo = furnitureJobOrderItem.JobNo,
                IsClosed = furnitureJobOrderItem.IsClosed,
                Remark = furnitureJobOrderItem.Remark
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderItems_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderItem furnitureJobOrderItem, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderItem
                {
                    JobTypeId = furnitureJobOrderItem.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderItem.UnitOfMeasurment,
                    Quantity = furnitureJobOrderItem.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderItem.UnitPriceBeforeVAT,
                    VAT = furnitureJobOrderItem.VAT,
                    UnitPriceWithVAT = furnitureJobOrderItem.UnitPriceWithVAT,
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                    FurnitureJobOrderFormId = id,
                    JobNo = furnitureJobOrderItem.JobNo,
                    Remark = furnitureJobOrderItem.Remark

                };

                db.FurnitureJobOrderItems.Add(entity);
                db.SaveChanges();
                furnitureJobOrderItem.FurnitureJobOrderItemId = entity.FurnitureJobOrderItemId;
            }

            return Json(new[] { furnitureJobOrderItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderItems_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderItem furnitureJobOrderItem)
        {
            var jobOrderItem = db.FurnitureJobOrderItems.Where(x => x.JobNo == furnitureJobOrderItem.JobNo).ToList();
            if (jobOrderItem.Any())
            {
                ModelState.AddModelError("", "This Job.No is already taken!");
            }
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderItem
                {
                    FurnitureJobOrderItemId = furnitureJobOrderItem.FurnitureJobOrderItemId,
                    JobTypeId = furnitureJobOrderItem.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderItem.UnitOfMeasurment,
                    Quantity = furnitureJobOrderItem.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderItem.UnitPriceBeforeVAT,
                    VAT = furnitureJobOrderItem.VAT,
                    UnitPriceWithVAT = furnitureJobOrderItem.UnitPriceWithVAT,
                    DateCreated = furnitureJobOrderItem.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = furnitureJobOrderItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    JobNo = furnitureJobOrderItem.JobNo,
                    FurnitureJobOrderFormId = furnitureJobOrderItem.FurnitureJobOrderFormId,
                    Remark = furnitureJobOrderItem.Remark
                };

                db.FurnitureJobOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderItems_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderItem furnitureJobOrderItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderItem
                {
                    FurnitureJobOrderItemId = furnitureJobOrderItem.FurnitureJobOrderItemId,
                    JobTypeId = furnitureJobOrderItem.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderItem.UnitOfMeasurment,
                    Quantity = furnitureJobOrderItem.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderItem.UnitPriceBeforeVAT,
                    VAT = furnitureJobOrderItem.VAT,
                    UnitPriceWithVAT = furnitureJobOrderItem.UnitPriceWithVAT,
                    DateCreated = furnitureJobOrderItem.DateCreated,
                    LastModified = furnitureJobOrderItem.LastModified,
                    CreatedBy = furnitureJobOrderItem.CreatedBy,
                    ModifiedBy = furnitureJobOrderItem.ModifiedBy,
                    Remark = furnitureJobOrderItem.Remark
                };

                db.FurnitureJobOrderItems.Attach(entity);
                db.FurnitureJobOrderItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
