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

        public ActionResult FurnitureBOQMaterials_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureBOQMaterial> furnitureboqmaterials = db.FurnitureBOQMaterials;
            DataSourceResult result = furnitureboqmaterials.ToDataSourceResult(request, furnitureBOQMaterial => new {
                FurnitureBOQMaterialId = furnitureBOQMaterial.FurnitureBOQMaterialId,
                ItemCode = furnitureBOQMaterial.ItemCode,
                UnitOfMeasurement = furnitureBOQMaterial.UnitOfMeasurement,
                Quantity = furnitureBOQMaterial.Quantity,
                Remark = furnitureBOQMaterial.Remark,
                WorkshopType = furnitureBOQMaterial.WorkshopType,
                DateCreated = furnitureBOQMaterial.DateCreated,
                LastModified = furnitureBOQMaterial.LastModified,
                CreatedBy = furnitureBOQMaterial.CreatedBy,
                ModifiedBy = furnitureBOQMaterial.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQMaterials_Create([DataSourceRequest]DataSourceRequest request, FurnitureBOQMaterial furnitureBOQMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQMaterial
                {
                    ItemCode = furnitureBOQMaterial.ItemCode,
                    UnitOfMeasurement = furnitureBOQMaterial.UnitOfMeasurement,
                    Quantity = furnitureBOQMaterial.Quantity,
                    Remark = furnitureBOQMaterial.Remark,
                    WorkshopType = furnitureBOQMaterial.WorkshopType,
                    DateCreated = furnitureBOQMaterial.DateCreated,
                    LastModified = furnitureBOQMaterial.LastModified,
                    CreatedBy = furnitureBOQMaterial.CreatedBy,
                    ModifiedBy = furnitureBOQMaterial.ModifiedBy
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
                    ModifiedBy = furnitureBOQMaterial.ModifiedBy
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
                    ModifiedBy = furnitureBOQMaterial.ModifiedBy
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
