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
                FurnitureProformaInvoiceId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceId,
                TotalPrice = furnitureProformaInvoiceItem.TotalPrice,
                TransportCost = furnitureProformaInvoiceItem.TransportCost,
                FurnitureEstimationId = furnitureProformaInvoiceItem.FurnitureEstimationId,
                IsActive = furnitureProformaInvoiceItem.IsActive,
                GLTaxRateID = furnitureProformaInvoiceItem.GLTaxRateID
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoiceItems_Create([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoiceItem model, int proformaId)
        {           
            var estimation = db.FurnitureEstimationForms.Find(model.FurnitureEstimationId);
            if (estimation == null) ModelState.AddModelError(string.Empty, "Can't find Estimation");

            var jobType = db.FurnitureStandardJobTypes.Find(estimation.JobTypeId);
            if (jobType == null) ModelState.AddModelError(string.Empty, "Job Type Not Found");

            var overAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
            if (overAllCost == null) ModelState.AddModelError(string.Empty, "Overall cost not found");
            var glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GLTaxRateID);
            if(glTaxRate == null)
            {
                ModelState.AddModelError(string.Empty, "Tax rate not found!");
            }
            if (ModelState.IsValid)
            {

                var entity = new FurnitureProformaInvoiceItem
                {
                    JobTypeId = estimation.JobTypeId,
                    UnitOfMeasurment = jobType.Unit,
                    Quantity = model.Quantity,
                    UnitPriceBeforeVAT = overAllCost.SellingPrice,
                    VAT = glTaxRate.CalculatedRate,
                    UnitPriceWithVAT = (overAllCost.SellingPrice * glTaxRate.CalculatedRate) + overAllCost.SellingPrice,
                    Remark = model.Remark,
                    DateCreated = DateTime.Today,                   
                    CreatedBy = User.Identity.Name,                    
                    FurnitureProformaInvoiceId = proformaId,
                    FurnitureEstimationId = model.FurnitureEstimationId,
                    TransportCost = model.TransportCost,
                    GLTaxRateID = model.GLTaxRateID
                };
                entity.TotalPrice = entity.UnitPriceWithVAT * entity.Quantity;
                db.FurnitureProformaInvoiceItems.Add(entity);
                db.SaveChanges();
                model.FurnitureProformaInvoiceItemId = entity.FurnitureProformaInvoiceItemId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoiceItems_Update([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoiceItem model)
        {
            var estimation = db.FurnitureEstimationForms.Find(model.FurnitureEstimationId);
            if (estimation == null) ModelState.AddModelError(string.Empty, "Can't find Estimation");

            var jobType = db.FurnitureStandardJobTypes.Find(estimation.JobTypeId);
            if (jobType == null) ModelState.AddModelError(string.Empty, "Job Type Not Found");

            var overAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
            if (overAllCost == null) ModelState.AddModelError(string.Empty, "Overall cost not found");
            var glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GLTaxRateID);
            if (glTaxRate == null)
            {
                ModelState.AddModelError(string.Empty, "Tax rate not found!");
            }
            if (ModelState.IsValid)
            {

                var entity = new FurnitureProformaInvoiceItem
                {
                    JobTypeId = estimation.JobTypeId,
                    UnitOfMeasurment = jobType.Unit,
                    Quantity = model.Quantity,
                    UnitPriceBeforeVAT = overAllCost.SellingPrice,
                    VAT = glTaxRate.CalculatedRate,
                    UnitPriceWithVAT = (overAllCost.SellingPrice * glTaxRate.CalculatedRate) + overAllCost.SellingPrice,

                    Remark = model.Remark,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    FurnitureProformaInvoiceId = model.FurnitureProformaInvoiceId,
                    FurnitureEstimationId = model.FurnitureEstimationId,
                    TransportCost = model.TransportCost,
                    GLTaxRateID = model.GLTaxRateID

                };
                entity.TotalPrice = entity.UnitPriceWithVAT * entity.Quantity;
                db.FurnitureProformaInvoiceItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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
                    FurnitureProformaInvoiceId = furnitureProformaInvoiceItem.FurnitureProformaInvoiceId,
                    TransportCost = furnitureProformaInvoiceItem.TransportCost
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
