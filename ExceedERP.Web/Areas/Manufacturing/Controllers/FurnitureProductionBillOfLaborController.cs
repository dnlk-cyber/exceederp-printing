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
    public class FurnitureProductionBillOfLaborController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureProductionBillOfLabors_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductionBillOfLabor> furnitureproductionbilloflabors = db.FurnitureProductionBillOfLabors.Where(x => x.FurnitureJobOrderProductionId == id);
            DataSourceResult result = furnitureproductionbilloflabors.ToDataSourceResult(request, furnitureProductionBillOfLabor => new FurnitureProductionBillOfLabor
            {
                FurnitureProductionBillOfLaborId = furnitureProductionBillOfLabor.FurnitureProductionBillOfLaborId,
                ManufacturingTaskCategoryId = furnitureProductionBillOfLabor.ManufacturingTaskCategoryId,
                WorkShopType = furnitureProductionBillOfLabor.WorkShopType,
                NumberOfEmloyees = furnitureProductionBillOfLabor.NumberOfEmloyees,
                EstimatedMinute = furnitureProductionBillOfLabor.EstimatedMinute,
                FurnitureJobOrderProductionId = furnitureProductionBillOfLabor.FurnitureJobOrderProductionId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfLabors_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfLabor furnitureProductionBillOfLabor, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfLabor
                {
                    ManufacturingTaskCategoryId = furnitureProductionBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = furnitureProductionBillOfLabor.WorkShopType,
                    NumberOfEmloyees = furnitureProductionBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureProductionBillOfLabor.EstimatedMinute,
                    FurnitureJobOrderProductionId = id
                };

                db.FurnitureProductionBillOfLabors.Add(entity);
                db.SaveChanges();
                furnitureProductionBillOfLabor.FurnitureProductionBillOfLaborId = entity.FurnitureProductionBillOfLaborId;
            }

            return Json(new[] { furnitureProductionBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfLabors_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfLabor furnitureProductionBillOfLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfLabor
                {
                    FurnitureProductionBillOfLaborId = furnitureProductionBillOfLabor.FurnitureProductionBillOfLaborId,
                    ManufacturingTaskCategoryId = furnitureProductionBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = furnitureProductionBillOfLabor.WorkShopType,
                    NumberOfEmloyees = furnitureProductionBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureProductionBillOfLabor.EstimatedMinute,
                };

                db.FurnitureProductionBillOfLabors.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionBillOfLabors_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionBillOfLabor furnitureProductionBillOfLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionBillOfLabor
                {
                    FurnitureProductionBillOfLaborId = furnitureProductionBillOfLabor.FurnitureProductionBillOfLaborId,
                    ManufacturingTaskCategoryId = furnitureProductionBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = furnitureProductionBillOfLabor.WorkShopType,
                    NumberOfEmloyees = furnitureProductionBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = furnitureProductionBillOfLabor.EstimatedMinute,
                };

                db.FurnitureProductionBillOfLabors.Attach(entity);
                db.FurnitureProductionBillOfLabors.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
