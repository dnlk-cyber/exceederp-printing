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
    public class FurnitureBOQLaborController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureBOQLabors_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureBOQLabor> furnitureboqlabors = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == id);
            DataSourceResult result = furnitureboqlabors.ToDataSourceResult(request, furnitureBOQLabor => new FurnitureBOQLabor
            {
                FurnitureBOQLaborId = furnitureBOQLabor.FurnitureBOQLaborId,
                WorkShopType = furnitureBOQLabor.WorkShopType,
                NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                EstimatedMinute = furnitureBOQLabor.EstimatedMinute,
                DateCreated = furnitureBOQLabor.DateCreated,
                LastModified = furnitureBOQLabor.LastModified,
                CreatedBy = furnitureBOQLabor.CreatedBy,
                ModifiedBy = furnitureBOQLabor.ModifiedBy,
                FurnitureBillOfQuantityId = furnitureBOQLabor.FurnitureBillOfQuantityId,
                ManufacturingTaskCategoryId = furnitureBOQLabor.ManufacturingTaskCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQLabors_Create([DataSourceRequest]DataSourceRequest request, FurnitureBOQLabor furnitureBOQLabor, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQLabor
                {
                    WorkShopType = furnitureBOQLabor.WorkShopType,
                    NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureBOQLabor.EstimatedMinute,
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                   
                    FurnitureBillOfQuantityId = id,
                    ManufacturingTaskCategoryId = furnitureBOQLabor.ManufacturingTaskCategoryId

                };

                db.FurnitureBOQLabors.Add(entity);
                db.SaveChanges();
                furnitureBOQLabor.FurnitureBOQLaborId = entity.FurnitureBOQLaborId;
            }

            return Json(new[] { furnitureBOQLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQLabors_Update([DataSourceRequest]DataSourceRequest request, FurnitureBOQLabor furnitureBOQLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQLabor
                {
                    FurnitureBOQLaborId = furnitureBOQLabor.FurnitureBOQLaborId,
                    WorkShopType = furnitureBOQLabor.WorkShopType,
                    NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureBOQLabor.EstimatedMinute,
                    DateCreated = furnitureBOQLabor.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = furnitureBOQLabor.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FurnitureBillOfQuantityId = furnitureBOQLabor.FurnitureBillOfQuantityId,
                    ManufacturingTaskCategoryId = furnitureBOQLabor.ManufacturingTaskCategoryId
                };

                db.FurnitureBOQLabors.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureBOQLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQLabors_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureBOQLabor furnitureBOQLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQLabor
                {
                    FurnitureBOQLaborId = furnitureBOQLabor.FurnitureBOQLaborId,
                    WorkShopType = furnitureBOQLabor.WorkShopType,
                    NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureBOQLabor.EstimatedMinute,
                    DateCreated = furnitureBOQLabor.DateCreated,
                    LastModified = furnitureBOQLabor.LastModified,
                    CreatedBy = furnitureBOQLabor.CreatedBy,
                    ModifiedBy = furnitureBOQLabor.ModifiedBy,
                    FurnitureBillOfQuantityId = furnitureBOQLabor.FurnitureBillOfQuantityId,
                    ManufacturingTaskCategoryId = furnitureBOQLabor.ManufacturingTaskCategoryId
                };

                db.FurnitureBOQLabors.Attach(entity);
                db.FurnitureBOQLabors.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureBOQLabor }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
