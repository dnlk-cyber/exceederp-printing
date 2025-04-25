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
using ExceedERP.Helpers.Printing;

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
            
            return Json(printingestimationmaterialcosts.ToDataSourceResult(request));
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost model, int estId)
        {            
            if (ModelState.IsValid)
            {
                var item = db.PrintingMaterialCategoryItems.Find(model.PrintingMaterialCategoryItemId);
                var entity = new PrintingEstimationMaterialCost
                {
                    PrintingMaterialCategoryId = model.PrintingMaterialCategoryId,
                    PrintingMaterialCategoryItemId = model.PrintingMaterialCategoryItemId,
                    Quantity = model.Quantity,                   
                    DateCreated = DateTime.Today,
                    LastModified = model.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = model.ModifiedBy,
                    PrintingCostEstimationId = estId,
                    WastageInPercentage = model.WastageInPercentage,
                    EstimationMaterialType = model.EstimationMaterialType
                };
                var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == estId);
                if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPlate(model, estId);
                            break;
                        case EstimationMaterialType.CoverPlate:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPlate(model, estId);
                            break;
                        case EstimationMaterialType.Glue:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateGlue(model, estId);
                            break;
                        case EstimationMaterialType.InkText:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateInkText(model, estId);
                            break;
                        case EstimationMaterialType.InkCover:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateInkCover(model, estId);
                            break;
                        default:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculatePackagingPaper(model, estId);
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Card)
                {
                    //switch (model.EstimationMaterialType)
                    //{
                    //    case EstimationMaterialType.TextPaper:
                    //        entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPaperQuantityCard(model, estId);
                    //        break;                       
                    //    case EstimationMaterialType.TextPlate:
                    //        entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPlateCard(model, estId);
                    //        break;                        
                    //}
                }
                if(entity.Quantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong! missing data on setting");
                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
                entity.UnitCost = item.UnitPrice;
                entity.UnitOfMeasurment = item.UnitOfMeasurement;
                entity.TotalCost = entity.UnitCost * entity.Quantity;

                model.Quantity = entity.Quantity;
                model.UnitCost = entity.UnitCost;
                model.UnitOfMeasurment = entity.UnitOfMeasurment;
                model.TotalCost = entity.TotalCost;
              
                db.PrintingEstimationMaterialCosts.Add(entity);
                db.SaveChanges();
                model.PrintingEstimationMaterialCostId = entity.PrintingEstimationMaterialCostId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost model)
        {
            if (ModelState.IsValid)
            {
                var item = db.PrintingMaterialCategoryItems.Find(model.PrintingMaterialCategoryItemId);

                var entity = new PrintingEstimationMaterialCost
                {
                    PrintingMaterialCategoryId = model.PrintingMaterialCategoryId,
                    PrintingMaterialCategoryItemId = model.PrintingMaterialCategoryItemId,
                    Quantity = model.Quantity,
                    DateCreated = model.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    PrintingCostEstimationId = model.PrintingCostEstimationId,
                    PrintingEstimationMaterialCostId = model.PrintingEstimationMaterialCostId
                };
                var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId);
                if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPlate(model, model.PrintingCostEstimationId);
                            break;
                        default:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPlate(model, model.PrintingCostEstimationId);
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Card)
                {
                    switch (model.EstimationMaterialType)
                    {
                        //case EstimationMaterialType.TextPaper:
                        //    entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPaperQuantityCard(model, model.PrintingCostEstimationId);
                        //    break;
                        //case EstimationMaterialType.TextPlate:
                        //    entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPlateCard(model, model.PrintingCostEstimationId);
                        //    break;
                    }
                }

                entity.UnitCost = item.UnitPrice;
                entity.UnitOfMeasurment = item.UnitOfMeasurement;
                entity.TotalCost = entity.UnitCost * entity.Quantity;


                db.PrintingEstimationMaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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
