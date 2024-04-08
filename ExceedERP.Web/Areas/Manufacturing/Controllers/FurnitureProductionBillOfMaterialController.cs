﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureProductionBillOfMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureProductionBillOfMaterials_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductionBillOfMaterial> furnitureproductionbillofmaterials = db.FurnitureProductionBillOfMaterials.Where(x => x.FurnitureJobOrderProductionId == id);
            DataSourceResult result = furnitureproductionbillofmaterials.ToDataSourceResult(request, furnitureProductionBillOfMaterial => new FurnitureProductionBillOfMaterial
            {
                FurnitureProductionBillOfMaterialId = furnitureProductionBillOfMaterial.FurnitureProductionBillOfMaterialId,
                ManufacturingMaterialCategoryId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryId,
                ManufacturingMaterialCategoryItemId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryItemId,
                ItemCode = furnitureProductionBillOfMaterial.ItemCode,
                UnitOfMeasurement = furnitureProductionBillOfMaterial.UnitOfMeasurement,
                Quantity = furnitureProductionBillOfMaterial.Quantity,
                Remark = furnitureProductionBillOfMaterial.Remark,
                WorkshopType = furnitureProductionBillOfMaterial.WorkshopType,
                FurnitureJobOrderProductionId = furnitureProductionBillOfMaterial.FurnitureJobOrderProductionId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfMaterials_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfMaterial furnitureProductionBillOfMaterial, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfMaterial
                {
                    ManufacturingMaterialCategoryId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = furnitureProductionBillOfMaterial.ItemCode,
                    UnitOfMeasurement = furnitureProductionBillOfMaterial.UnitOfMeasurement,
                    Quantity = furnitureProductionBillOfMaterial.Quantity,
                    Remark = furnitureProductionBillOfMaterial.Remark,
                    WorkshopType = furnitureProductionBillOfMaterial.WorkshopType,
                    FurnitureJobOrderProductionId = id
                };

                db.FurnitureProductionBillOfMaterials.Add(entity);
                db.SaveChanges();
                furnitureProductionBillOfMaterial.FurnitureProductionBillOfMaterialId = entity.FurnitureProductionBillOfMaterialId;
            }

            return Json(new[] { furnitureProductionBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfMaterials_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfMaterial furnitureProductionBillOfMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfMaterial
                {
                    FurnitureProductionBillOfMaterialId = furnitureProductionBillOfMaterial.FurnitureProductionBillOfMaterialId,
                    ManufacturingMaterialCategoryId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = furnitureProductionBillOfMaterial.ItemCode,
                    UnitOfMeasurement = furnitureProductionBillOfMaterial.UnitOfMeasurement,
                    Quantity = furnitureProductionBillOfMaterial.Quantity,
                    Remark = furnitureProductionBillOfMaterial.Remark,
                    WorkshopType = furnitureProductionBillOfMaterial.WorkshopType,
                };

                db.FurnitureProductionBillOfMaterials.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfMaterials_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfMaterial furnitureProductionBillOfMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfMaterial
                {
                    FurnitureProductionBillOfMaterialId = furnitureProductionBillOfMaterial.FurnitureProductionBillOfMaterialId,
                    ManufacturingMaterialCategoryId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = furnitureProductionBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = furnitureProductionBillOfMaterial.ItemCode,
                    UnitOfMeasurement = furnitureProductionBillOfMaterial.UnitOfMeasurement,
                    Quantity = furnitureProductionBillOfMaterial.Quantity,
                    Remark = furnitureProductionBillOfMaterial.Remark,
                    WorkshopType = furnitureProductionBillOfMaterial.WorkshopType,
                };

                db.FurnitureProductionBillOfMaterials.Attach(entity);
                db.FurnitureProductionBillOfMaterials.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
