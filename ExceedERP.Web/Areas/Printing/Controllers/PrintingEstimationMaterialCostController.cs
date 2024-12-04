﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingEstimationMaterialCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingEstimationMaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int estId)
        {
            IQueryable<PrintingEstimationMaterialCost> printingestimationmaterialcosts = db.PrintingEstimationMaterialCosts.Where(x => x.PrintingCostEstimationId == estId);
            DataSourceResult result = printingestimationmaterialcosts.ToDataSourceResult(request, printingEstimationMaterialCost => new PrintingEstimationMaterialCost
            {
                PrintingEstimationMaterialCostId = printingEstimationMaterialCost.PrintingEstimationMaterialCostId,
                PrintingMaterialCategoryId = printingEstimationMaterialCost.PrintingMaterialCategoryId,
                PrintingMaterialCategoryItemId = printingEstimationMaterialCost.PrintingMaterialCategoryItemId,
                Quantity = printingEstimationMaterialCost.Quantity,
                UnitCost = printingEstimationMaterialCost.UnitCost,
                TotalCost = printingEstimationMaterialCost.TotalCost,
                UnitOfMeasurment = printingEstimationMaterialCost.UnitOfMeasurment,
                DateCreated = printingEstimationMaterialCost.DateCreated,
                LastModified = printingEstimationMaterialCost.LastModified,
                CreatedBy = printingEstimationMaterialCost.CreatedBy,
                ModifiedBy = printingEstimationMaterialCost.ModifiedBy,
                PrintingCostEstimationId = printingEstimationMaterialCost.PrintingCostEstimationId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost printingEstimationMaterialCost, int estId)
        {            
            if (ModelState.IsValid)
            {
                var item = db.PrintingMaterialCategoryItems.Find(printingEstimationMaterialCost.PrintingMaterialCategoryItemId);
                var entity = new PrintingEstimationMaterialCost
                {
                    PrintingMaterialCategoryId = printingEstimationMaterialCost.PrintingMaterialCategoryId,
                    PrintingMaterialCategoryItemId = printingEstimationMaterialCost.PrintingMaterialCategoryItemId,
                    Quantity = printingEstimationMaterialCost.Quantity,                   
                    DateCreated = DateTime.Today,
                    LastModified = printingEstimationMaterialCost.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingEstimationMaterialCost.ModifiedBy,
                    PrintingCostEstimationId = estId
                };

                entity.UnitCost = item.UnitPrice;
                entity.UnitOfMeasurment = item.UnitOfMeasurement;
                entity.TotalCost = entity.UnitCost * entity.Quantity;

                printingEstimationMaterialCost.UnitCost = entity.UnitCost;
                printingEstimationMaterialCost.UnitOfMeasurment = entity.UnitOfMeasurment;
                printingEstimationMaterialCost.TotalCost = entity.TotalCost;

                db.PrintingEstimationMaterialCosts.Add(entity);
                db.SaveChanges();
                printingEstimationMaterialCost.PrintingEstimationMaterialCostId = entity.PrintingEstimationMaterialCostId;
            }

            return Json(new[] { printingEstimationMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost printingEstimationMaterialCost)
        {
            if (ModelState.IsValid)
            {
                var item = db.PrintingMaterialCategoryItems.Find(printingEstimationMaterialCost.PrintingMaterialCategoryItemId);

                var entity = new PrintingEstimationMaterialCost
                {
                    PrintingMaterialCategoryId = printingEstimationMaterialCost.PrintingMaterialCategoryId,
                    PrintingMaterialCategoryItemId = printingEstimationMaterialCost.PrintingMaterialCategoryItemId,
                    Quantity = printingEstimationMaterialCost.Quantity,
                    DateCreated = DateTime.Today,
                    LastModified = printingEstimationMaterialCost.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingEstimationMaterialCost.ModifiedBy,
                    PrintingCostEstimationId = printingEstimationMaterialCost.PrintingCostEstimationId,
                    PrintingEstimationMaterialCostId = printingEstimationMaterialCost.PrintingEstimationMaterialCostId
                };
                entity.UnitCost = item.UnitPrice;
                entity.UnitOfMeasurment = item.UnitOfMeasurement;
                entity.TotalCost = entity.UnitCost * entity.Quantity;

                db.PrintingEstimationMaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Destroy([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost printingEstimationMaterialCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationMaterialCost
                {
                    PrintingEstimationMaterialCostId = printingEstimationMaterialCost.PrintingEstimationMaterialCostId,
                    PrintingMaterialCategoryId = printingEstimationMaterialCost.PrintingMaterialCategoryId,
                    PrintingMaterialCategoryItemId = printingEstimationMaterialCost.PrintingMaterialCategoryItemId,
                    Quantity = printingEstimationMaterialCost.Quantity,
                    UnitCost = printingEstimationMaterialCost.UnitCost,
                    TotalCost = printingEstimationMaterialCost.TotalCost,
                    UnitOfMeasurment = printingEstimationMaterialCost.UnitOfMeasurment,
                    DateCreated = printingEstimationMaterialCost.DateCreated,
                    LastModified = printingEstimationMaterialCost.LastModified,
                    CreatedBy = printingEstimationMaterialCost.CreatedBy,
                    ModifiedBy = printingEstimationMaterialCost.ModifiedBy,
                    PrintingCostEstimationId = printingEstimationMaterialCost.PrintingCostEstimationId
                };

                db.PrintingEstimationMaterialCosts.Attach(entity);
                db.PrintingEstimationMaterialCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationMaterialCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
