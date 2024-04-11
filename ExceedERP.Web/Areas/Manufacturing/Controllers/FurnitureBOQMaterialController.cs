﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureBOQMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureBOQMaterials_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureBOQMaterial> furnitureboqmaterials = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == id).OrderByDescending(x => x.WorkshopType);
            DataSourceResult result = furnitureboqmaterials.ToDataSourceResult(request, furnitureBOQMaterial => new FurnitureBOQMaterial
            {
                FurnitureBOQMaterialId = furnitureBOQMaterial.FurnitureBOQMaterialId,
                ItemCode = furnitureBOQMaterial.ItemCode,
                UnitOfMeasurement = furnitureBOQMaterial.UnitOfMeasurement,
                Quantity = furnitureBOQMaterial.Quantity,
                Remark = furnitureBOQMaterial.Remark,
                WorkshopType = furnitureBOQMaterial.WorkshopType,
                DateCreated = furnitureBOQMaterial.DateCreated,
                LastModified = furnitureBOQMaterial.LastModified,
                CreatedBy = furnitureBOQMaterial.CreatedBy,
                ModifiedBy = furnitureBOQMaterial.ModifiedBy,
                FurnitureBillOfQuantityId = furnitureBOQMaterial.FurnitureBillOfQuantityId,
                ManufacturingMaterialCategoryId = furnitureBOQMaterial.ManufacturingMaterialCategoryId,
                ManufacturingMaterialCategoryItemId = furnitureBOQMaterial.ManufacturingMaterialCategoryItemId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQMaterials_Create([DataSourceRequest]DataSourceRequest request, FurnitureBOQMaterial furnitureBOQMaterial, int id)
        {
            
            var materialItems = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == furnitureBOQMaterial.ManufacturingMaterialCategoryItemId);
            if (materialItems == null) ModelState.AddModelError("", "Item Not Found");
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQMaterial
                {
                    ItemCode = furnitureBOQMaterial.ItemCode,
                    UnitOfMeasurement = materialItems.UnitOfMeasurement,
                    Quantity = furnitureBOQMaterial.Quantity,
                    Remark = furnitureBOQMaterial.Remark,
                    WorkshopType = furnitureBOQMaterial.WorkshopType,
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                    
                    FurnitureBillOfQuantityId = id,
                    ManufacturingMaterialCategoryItemId = furnitureBOQMaterial.ManufacturingMaterialCategoryItemId,
                    ManufacturingMaterialCategoryId = furnitureBOQMaterial.ManufacturingMaterialCategoryId,
                    
                };

                db.FurnitureBOQMaterials.Add(entity);
                db.SaveChanges();
                furnitureBOQMaterial.FurnitureBOQMaterialId = entity.FurnitureBOQMaterialId;
            }

            return Json(new[] { furnitureBOQMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQMaterials_Update([DataSourceRequest]DataSourceRequest request, FurnitureBOQMaterial furnitureBOQMaterial)
        {
            var materialItems = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == furnitureBOQMaterial.ManufacturingMaterialCategoryItemId);
            if (materialItems == null) ModelState.AddModelError("", "Item Not Found");
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQMaterial
                {
                    FurnitureBOQMaterialId = furnitureBOQMaterial.FurnitureBOQMaterialId,
                    ItemCode = furnitureBOQMaterial.ItemCode,
                    UnitOfMeasurement = materialItems.UnitOfMeasurement,
                    Quantity = furnitureBOQMaterial.Quantity,
                    Remark = furnitureBOQMaterial.Remark,
                    WorkshopType = furnitureBOQMaterial.WorkshopType,
                    DateCreated = furnitureBOQMaterial.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = furnitureBOQMaterial.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ManufacturingMaterialCategoryItemId = furnitureBOQMaterial.ManufacturingMaterialCategoryItemId,
                    ManufacturingMaterialCategoryId = furnitureBOQMaterial.ManufacturingMaterialCategoryId,
                    FurnitureBillOfQuantityId = furnitureBOQMaterial.FurnitureBillOfQuantityId
                };

                db.FurnitureBOQMaterials.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureBOQMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQMaterials_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureBOQMaterial furnitureBOQMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQMaterial
                {
                    FurnitureBOQMaterialId = furnitureBOQMaterial.FurnitureBOQMaterialId,
                    ItemCode = furnitureBOQMaterial.ItemCode,
                    UnitOfMeasurement = furnitureBOQMaterial.UnitOfMeasurement,
                    Quantity = furnitureBOQMaterial.Quantity,
                    Remark = furnitureBOQMaterial.Remark,
                    WorkshopType = furnitureBOQMaterial.WorkshopType,
                    DateCreated = furnitureBOQMaterial.DateCreated,
                    LastModified = furnitureBOQMaterial.LastModified,
                    CreatedBy = furnitureBOQMaterial.CreatedBy,
                    ModifiedBy = furnitureBOQMaterial.ModifiedBy,
                    ManufacturingMaterialCategoryItemId = furnitureBOQMaterial.ManufacturingMaterialCategoryItemId,
                    ManufacturingMaterialCategoryId = furnitureBOQMaterial.ManufacturingMaterialCategoryId,
                };

                db.FurnitureBOQMaterials.Attach(entity);
                db.FurnitureBOQMaterials.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureBOQMaterial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
