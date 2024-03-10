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
    public class FurnitureProformaInvoiceItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureProformaInvoiceItems_Read([DataSourceRequest]DataSourceRequest request, int proformaId)
        {
            IQueryable<FurnitureProformaInvoiceItem> furnitureproformainvoiceitems = db.FurnitureProformaInvoiceItems.Where(x => x.FurnitureProformaInvoiceId == proformaId);
            DataSourceResult result = furnitureproformainvoiceitems.ToDataSourceResult(request, furnitureProformaInvoiceItem => new FurnitureProformaInvoiceItem
            {
                FurnitureProformaInvoiceItemId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceItemId,
                JobTypeId = furnitureProformaInvoiceItem.JobTypeId,
                UnitOfMeasurment = furnitureProformaInvoiceItem.UnitOfMeasurment,
                Quantity = furnitureProformaInvoiceItem.Quantity,
                UnitPriceBeforeVAT = furnitureProformaInvoiceItem.UnitPriceBeforeVAT,
                VAT = furnitureProformaInvoiceItem.VAT,
                UnitPriceWithVAT = furnitureProformaInvoiceItem.UnitPriceWithVAT,
                Remark = furnitureProformaInvoiceItem.Remark,
                DateCreated = furnitureProformaInvoiceItem.DateCreated,
                LastModified = furnitureProformaInvoiceItem.LastModified,
                CreatedBy = furnitureProformaInvoiceItem.CreatedBy,
                ModifiedBy = furnitureProformaInvoiceItem.ModifiedBy,
                FurnitureProformaInvoiceId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoiceItems_Create([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoiceItem furnitureProformaInvoiceItem, int proformaId)
        {
            var jobCategory = db.JobCategories.Find(furnitureProformaInvoiceItem.JobTypeId);
            if (jobCategory == null) ModelState.AddModelError(string.Empty, "Job Type Not Found");
            var taxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobCategory.GLTaxRateID);
            if (taxRate == null) ModelState.AddModelError(string.Empty, "Tax Rate Not Found");
            if (ModelState.IsValid)
            {
                
                var entity = new FurnitureProformaInvoiceItem
                {
                    JobTypeId = furnitureProformaInvoiceItem.JobTypeId,
                    UnitOfMeasurment = furnitureProformaInvoiceItem.UnitOfMeasurment,
                    Quantity = furnitureProformaInvoiceItem.Quantity,
                    UnitPriceBeforeVAT = furnitureProformaInvoiceItem.UnitPriceBeforeVAT,
                    VAT = taxRate.CalculatedRate,
                    UnitPriceWithVAT = furnitureProformaInvoiceItem.UnitPriceWithVAT,
                    Remark = furnitureProformaInvoiceItem.Remark,
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                    
                    FurnitureProformaInvoiceId = proformaId
                };
                entity.UnitPriceWithVAT = entity.UnitPriceBeforeVAT + entity.UnitPriceBeforeVAT * entity.VAT;
                db.FurnitureProformaInvoiceItems.Add(entity);
                db.SaveChanges();
                furnitureProformaInvoiceItem.FurnitureProformaInvoiceItemId = entity.FurnitureProformaInvoiceItemId;
            }

            return Json(new[] { furnitureProformaInvoiceItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoiceItems_Update([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoiceItem furnitureProformaInvoiceItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProformaInvoiceItem
                {
                    FurnitureProformaInvoiceItemId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceItemId,
                    JobTypeId = furnitureProformaInvoiceItem.JobTypeId,
                    UnitOfMeasurment = furnitureProformaInvoiceItem.UnitOfMeasurment,
                    Quantity = furnitureProformaInvoiceItem.Quantity,
                    UnitPriceBeforeVAT = furnitureProformaInvoiceItem.UnitPriceBeforeVAT,
                    VAT = furnitureProformaInvoiceItem.VAT,
                    UnitPriceWithVAT = furnitureProformaInvoiceItem.UnitPriceWithVAT,
                    Remark = furnitureProformaInvoiceItem.Remark,
                    DateCreated = furnitureProformaInvoiceItem.DateCreated,
                    LastModified = furnitureProformaInvoiceItem.LastModified,
                    CreatedBy = furnitureProformaInvoiceItem.CreatedBy,
                    ModifiedBy = furnitureProformaInvoiceItem.ModifiedBy,
                    FurnitureProformaInvoiceId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceId
                };

                db.FurnitureProformaInvoiceItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureProformaInvoiceItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoiceItems_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoiceItem furnitureProformaInvoiceItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProformaInvoiceItem
                {
                    FurnitureProformaInvoiceItemId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceItemId,
                    JobTypeId = furnitureProformaInvoiceItem.JobTypeId,
                    UnitOfMeasurment = furnitureProformaInvoiceItem.UnitOfMeasurment,
                    Quantity = furnitureProformaInvoiceItem.Quantity,
                    UnitPriceBeforeVAT = furnitureProformaInvoiceItem.UnitPriceBeforeVAT,
                    VAT = furnitureProformaInvoiceItem.VAT,
                    UnitPriceWithVAT = furnitureProformaInvoiceItem.UnitPriceWithVAT,
                    Remark = furnitureProformaInvoiceItem.Remark,
                    DateCreated = furnitureProformaInvoiceItem.DateCreated,
                    LastModified = furnitureProformaInvoiceItem.LastModified,
                    CreatedBy = furnitureProformaInvoiceItem.CreatedBy,
                    ModifiedBy = furnitureProformaInvoiceItem.ModifiedBy,
                    FurnitureProformaInvoiceId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceId
                };

                db.FurnitureProformaInvoiceItems.Attach(entity);
                db.FurnitureProformaInvoiceItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureProformaInvoiceItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
