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
    public class PrintingOverAllCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingOverAllCosts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingOverAllCost> printingoverallcosts = db.PrintingOverAllCosts.Where(x => x.PrintingCostEstimationId == id);        
            return Json(printingoverallcosts.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingOverAllCosts_Create([DataSourceRequest]DataSourceRequest request, PrintingOverAllCost printingOverAllCost, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingOverAllCost
                {
                    MaterialCost = printingOverAllCost.MaterialCost,
                    LaborCost = printingOverAllCost.LaborCost,
                    TotalProductionCost = printingOverAllCost.TotalProductionCost,
                    AdminstrativeCost = printingOverAllCost.AdminstrativeCost,
                    ProfitMargin = printingOverAllCost.ProfitMargin,
                    GraphicsCost = printingOverAllCost.GraphicsCost,
                    
                    GrandTotal = printingOverAllCost.GrandTotal,
                    PrintingCostEstimationId = id
                };

                db.PrintingOverAllCosts.Add(entity);
                db.SaveChanges();
                printingOverAllCost.PrintingOverAllCostId = entity.PrintingOverAllCostId;
            }

            return Json(new[] { printingOverAllCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingOverAllCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingOverAllCost model)
        {
            var estimation = db.PrintingCostEstimations.Find(model.PrintingCostEstimationId);
            var textPaperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
            if (estimation == null || textPaperSize == null)
            {
                ModelState.AddModelError(string.Empty, " Somethings wrong! Estimation or paper size is missing");
            }
            if (ModelState.IsValid)
            {
                model.AdminRate = model.AdminRate / 100;
                model.OverHeadRate = model.OverHeadRate / 100;
                var entity = new PrintingOverAllCost
                {
                    PrintingOverAllCostId = model.PrintingOverAllCostId,
                    MaterialCost = model.MaterialCost,
                    LaborCost = model.LaborCost,
                    OverHeadCost = model.LaborCost * model.OverHeadRate,
                    //AdminstrativeCost = (model.MaterialCost + model.LaborCost + model.OverHeadCost) * model.AdminstrativeCost,
                    GraphicsCost = model.GraphicsCost,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    GraphicsRate = model.GraphicsRate,
                    AdminRate = model.AdminRate,
                    OverHeadRate = model.OverHeadRate                    
                };
                entity.AdminstrativeCost = (model.MaterialCost + model.LaborCost + entity.OverHeadCost) * model.AdminRate;
                decimal textArea = (decimal)textPaperSize.Length * (decimal)textPaperSize.Width;
                entity.GraphicsCost = (entity.GraphicsRate * textArea * estimation.NumberOfPagesForDesign);
                entity.TotalProductionCost = entity.MaterialCost + entity.LaborCost + entity.OverHeadCost;
                entity.MaterialHandlingStorageWaste = entity.TotalProductionCost * 0.025m;
                entity.GrandTotal = entity.TotalProductionCost + entity.AdminstrativeCost + entity.GraphicsCost + entity.MaterialHandlingStorageWaste;

                model.GrandTotal = entity.GrandTotal;
                model.AdminstrativeCost = entity.AdminstrativeCost;
                model.OverHeadCost = entity.OverHeadCost;
                model.TotalProductionCost = entity.TotalProductionCost;
                model.GraphicsCost = entity.GraphicsCost;
                db.PrintingOverAllCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingOverAllCosts_Destroy([DataSourceRequest]DataSourceRequest request, PrintingOverAllCost model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingOverAllCost
                {
                    PrintingOverAllCostId = model.PrintingOverAllCostId,
                    MaterialCost = model.MaterialCost,
                    LaborCost = model.LaborCost,
                    TotalProductionCost = model.TotalProductionCost,
                    AdminstrativeCost = model.AdminstrativeCost,
                    ProfitMargin = model.ProfitMargin,
                    GraphicsCost = model.GraphicsCost,
                    OverHeadRate = model.OverHeadRate,
                    AdminRate = model.AdminRate,
                    OverHeadCost = model.OverHeadCost,
                    GraphicsRate = model.GraphicsRate,
                    IndirectMaterialCost = model.IndirectMaterialCost,
                    MaterialHandlingStorageWaste = model.MaterialHandlingStorageWaste,
                    SellingPrice = model.SellingPrice,
                    GrandTotal = model.GrandTotal,
                    PrintingCostEstimationId = model.PrintingCostEstimationId
                };

                db.PrintingOverAllCosts.Attach(entity);
                db.PrintingOverAllCosts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
