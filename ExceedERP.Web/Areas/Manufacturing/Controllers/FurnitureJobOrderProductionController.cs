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
    public class FurnitureJobOrderProductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["ItemCode"] = db.InventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();

            ViewData["MaterialCategory"] = db.ManufacturingMaterialCategories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();

            ViewData["MaterialCategoryItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();

            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });

            ViewData["Employee"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstName,
                Value = s.EmployeeId
            });
            return View();
        }

        public ActionResult FurnitureJobOrderProductions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderProduction> furniturejoborderproductions = db.FurnitureJobOrderProductions;
            DataSourceResult result = furniturejoborderproductions.ToDataSourceResult(request, furnitureJobOrderProduction => new {
                FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                CustomerId = furnitureJobOrderProduction.CustomerId,
                JobTypeId = furnitureJobOrderProduction.JobTypeId,
                JobNo = furnitureJobOrderProduction.JobNo,
                Unit = furnitureJobOrderProduction.Unit,
                Quantity = furnitureJobOrderProduction.Quantity,
                DeliveryDate = furnitureJobOrderProduction.DeliveryDate
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Add(entity);
                db.SaveChanges();
                furnitureJobOrderProduction.FurnitureJobOrderProductionId = entity.FurnitureJobOrderProductionId;
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Attach(entity);
                db.FurnitureJobOrderProductions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
