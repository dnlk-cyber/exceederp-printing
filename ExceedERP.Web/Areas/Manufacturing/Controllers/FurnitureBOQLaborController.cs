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

        public ActionResult FurnitureBOQLabors_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureBOQLabor> furnitureboqlabors = db.FurnitureBOQLabors;
            DataSourceResult result = furnitureboqlabors.ToDataSourceResult(request, furnitureBOQLabor => new {
                FurnitureBOQLaborId = furnitureBOQLabor.FurnitureBOQLaborId,
                WorkShopType = furnitureBOQLabor.WorkShopType,
                NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                EstimatedHour = furnitureBOQLabor.EstimatedHour,
                DateCreated = furnitureBOQLabor.DateCreated,
                LastModified = furnitureBOQLabor.LastModified,
                CreatedBy = furnitureBOQLabor.CreatedBy,
                ModifiedBy = furnitureBOQLabor.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBOQLabors_Create([DataSourceRequest]DataSourceRequest request, FurnitureBOQLabor furnitureBOQLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBOQLabor
                {
                    WorkShopType = furnitureBOQLabor.WorkShopType,
                    NumberOfEmloyees = furnitureBOQLabor.NumberOfEmloyees,
                    EstimatedHour = furnitureBOQLabor.EstimatedHour,
                    DateCreated = furnitureBOQLabor.DateCreated,
                    LastModified = furnitureBOQLabor.LastModified,
                    CreatedBy = furnitureBOQLabor.CreatedBy,
                    ModifiedBy = furnitureBOQLabor.ModifiedBy
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
                    EstimatedHour = furnitureBOQLabor.EstimatedHour,
                    DateCreated = furnitureBOQLabor.DateCreated,
                    LastModified = furnitureBOQLabor.LastModified,
                    CreatedBy = furnitureBOQLabor.CreatedBy,
                    ModifiedBy = furnitureBOQLabor.ModifiedBy
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
                    EstimatedHour = furnitureBOQLabor.EstimatedHour,
                    DateCreated = furnitureBOQLabor.DateCreated,
                    LastModified = furnitureBOQLabor.LastModified,
                    CreatedBy = furnitureBOQLabor.CreatedBy,
                    ModifiedBy = furnitureBOQLabor.ModifiedBy
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
