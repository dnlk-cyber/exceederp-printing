﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class StandardBillOfMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StandardBillOfMaterials_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StandardBillOfMaterial> standardbillofmaterials = db.StandardBillOfMaterials.Where(x => x.FurnitureStandardJobTypeId == id);
            DataSourceResult result = standardbillofmaterials.ToDataSourceResult(request, standardBillOfMaterial => new StandardBillOfMaterial
            {
                StandardBillOfMaterialId = standardBillOfMaterial.StandardBillOfMaterialId,
                
                ManufacturingMaterialCategoryId = standardBillOfMaterial.ManufacturingMaterialCategoryId,
                ManufacturingMaterialCategoryItemId = standardBillOfMaterial.ManufacturingMaterialCategoryItemId,
                ItemCode = standardBillOfMaterial.ItemCode,
                UnitOfMeasurement = standardBillOfMaterial.UnitOfMeasurement,
                Quantity = standardBillOfMaterial.Quantity,
                Remark = standardBillOfMaterial.Remark,
                WorkshopType = standardBillOfMaterial.WorkshopType,
                DateCreated = standardBillOfMaterial.DateCreated,
                LastModified = standardBillOfMaterial.LastModified,
                CreatedBy = standardBillOfMaterial.CreatedBy,
                ModifiedBy = standardBillOfMaterial.ModifiedBy,
                FurnitureStandardJobTypeId = standardBillOfMaterial.FurnitureStandardJobTypeId
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfMaterials_Create([DataSourceRequest]DataSourceRequest request, StandardBillOfMaterial standardBillOfMaterial, int id)
        {
            var materialItems = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == standardBillOfMaterial.ManufacturingMaterialCategoryItemId);
            if (materialItems == null) ModelState.AddModelError("", "Item Not Found");
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfMaterial
                {
                    
                    ManufacturingMaterialCategoryId = standardBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = standardBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = standardBillOfMaterial.ItemCode,
                    UnitOfMeasurement = materialItems.UnitOfMeasurement,
                    Quantity = standardBillOfMaterial.Quantity,
                    Remark = standardBillOfMaterial.Remark,
                    WorkshopType = standardBillOfMaterial.WorkshopType,
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                    FurnitureStandardJobTypeId = id
                    
                };

                db.StandardBillOfMaterials.Add(entity);
                db.SaveChanges();
                standardBillOfMaterial.StandardBillOfMaterialId = entity.StandardBillOfMaterialId;
            }

            return Json(new[] { standardBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfMaterials_Update([DataSourceRequest]DataSourceRequest request, StandardBillOfMaterial standardBillOfMaterial)
        {
            var materialItems = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == standardBillOfMaterial.ManufacturingMaterialCategoryItemId);
            if (materialItems == null) ModelState.AddModelError("", "Item Not Found");
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfMaterial
                {
                    StandardBillOfMaterialId = standardBillOfMaterial.StandardBillOfMaterialId,
                    
                    ManufacturingMaterialCategoryId = standardBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = standardBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = standardBillOfMaterial.ItemCode,
                    UnitOfMeasurement = materialItems.UnitOfMeasurement,
                    Quantity = standardBillOfMaterial.Quantity,
                    Remark = standardBillOfMaterial.Remark,
                    WorkshopType = standardBillOfMaterial.WorkshopType,
                    DateCreated = standardBillOfMaterial.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = standardBillOfMaterial.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FurnitureStandardJobTypeId = standardBillOfMaterial.FurnitureStandardJobTypeId
                };

                db.StandardBillOfMaterials.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { standardBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfMaterials_Destroy([DataSourceRequest]DataSourceRequest request, StandardBillOfMaterial standardBillOfMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfMaterial
                {
                    StandardBillOfMaterialId = standardBillOfMaterial.StandardBillOfMaterialId,
                    
                    ManufacturingMaterialCategoryId = standardBillOfMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = standardBillOfMaterial.ManufacturingMaterialCategoryItemId,
                    ItemCode = standardBillOfMaterial.ItemCode,
                    UnitOfMeasurement = standardBillOfMaterial.UnitOfMeasurement,
                    Quantity = standardBillOfMaterial.Quantity,
                    Remark = standardBillOfMaterial.Remark,
                    WorkshopType = standardBillOfMaterial.WorkshopType,
                    DateCreated = standardBillOfMaterial.DateCreated,
                    LastModified = standardBillOfMaterial.LastModified,
                    CreatedBy = standardBillOfMaterial.CreatedBy,
                    ModifiedBy = standardBillOfMaterial.ModifiedBy,
                    FurnitureStandardJobTypeId = standardBillOfMaterial.FurnitureStandardJobTypeId
                    
                };

                db.StandardBillOfMaterials.Attach(entity);
                db.StandardBillOfMaterials.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { standardBillOfMaterial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
