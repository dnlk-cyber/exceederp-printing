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
            var existingMaterialCost = db.PrintingEstimationMaterialCosts.SingleOrDefault(x => x.PrintingCostEstimationId == estId && x.EstimationMaterialType == model.EstimationMaterialType);
            if (existingMaterialCost != null)
                ModelState.AddModelError("", $"{existingMaterialCost.EstimationMaterialType.ToString()} Exists!");
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
                if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine || costEstimation.PrintJobType == PrintJobType.Newsletter)
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
                        case EstimationMaterialType.WrappingPaper:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculatePackagingPaper(model, estId);
                            break;
                        case EstimationMaterialType.StitchingWire:
                            entity.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateStitchWire(model, estId);
                            break;
                        default:
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Flyer ||
                    costEstimation.PrintJobType == PrintJobType.Format || costEstimation.PrintJobType == PrintJobType.Poster || costEstimation.PrintJobType == PrintJobType.Sticker)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)CardFlyerMaterialCost.CalculateTextPaperQuantity(model, estId);
                            break;                        
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)CardFlyerMaterialCost.CalculatePlate(model, estId);
                            break;                       
                        case EstimationMaterialType.InkText:
                            entity.Quantity = (decimal)CardFlyerMaterialCost.CalculateInkText(model, estId);
                            break;                        
                        case EstimationMaterialType.WrappingPaper:
                            entity.Quantity = (decimal)CardFlyerMaterialCost.CalculatePackagingPaper(model, estId);
                            break;
                        default:
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Voucher)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateTextPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateCoverPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateTextPlate(model, estId);
                            break;
                        case EstimationMaterialType.CoverPlate:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateCoverPlate(model, estId);
                            break;
                        case EstimationMaterialType.Glue:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateGlue(model, estId);
                            break;
                        case EstimationMaterialType.InkText:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateInkText(model, estId);
                            break;
                        case EstimationMaterialType.InkCover:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateInkCover(model, estId);
                            break;
                        case EstimationMaterialType.WrappingPaper:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculatePackagingPaper(model, estId);
                            break;
                        case EstimationMaterialType.StitchingWire:
                            entity.Quantity = (decimal)VoucherMaterialCost.CalculateStitchWire(model, estId);
                            break;
                        default:
                            break;
                    }
                }
                if (entity.Quantity == -1)
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
            var existingMaterialCost = db.PrintingEstimationMaterialCosts
                .SingleOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId
                && x.EstimationMaterialType == model.EstimationMaterialType
                && x.PrintingEstimationMaterialCostId == model.PrintingEstimationMaterialCostId);
            if (existingMaterialCost != null)
                ModelState.AddModelError("", $"{existingMaterialCost.EstimationMaterialType.ToString()} Exists!");
            if (ModelState.IsValid)
            {
                var item = db.PrintingMaterialCategoryItems.Find(model.PrintingMaterialCategoryItemId);
                
                var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId);
                if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine || costEstimation.PrintJobType == PrintJobType.Newsletter)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateTextPlate(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPlate:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateCoverPlate(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.Glue:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateGlue(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.InkText:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateInkText(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.InkCover:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateInkCover(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.WrappingPaper:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.StitchingWire:
                            model.Quantity = (decimal)BookMagazineMaterialCostEstimation.CalculateStitchWire(model, model.PrintingCostEstimationId);
                            break;
                        default:
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Flyer ||
                    costEstimation.PrintJobType == PrintJobType.Format || costEstimation.PrintJobType == PrintJobType.Poster || costEstimation.PrintJobType == PrintJobType.Sticker)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            model.Quantity = (decimal)CardFlyerMaterialCost.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            model.Quantity = (decimal)CardFlyerMaterialCost.CalculatePlate(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.InkText:
                            model.Quantity = (decimal)CardFlyerMaterialCost.CalculateInkText(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.WrappingPaper:
                            model.Quantity = (decimal)CardFlyerMaterialCost.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                            break;
                        default:
                            break;
                    }
                }
                else if (costEstimation.PrintJobType == PrintJobType.Voucher)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateTextPlate(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPlate:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateCoverPlate(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.Glue:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateGlue(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.InkText:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateInkText(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.InkCover:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateInkCover(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.WrappingPaper:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.StitchingWire:
                            model.Quantity = (decimal)VoucherMaterialCost.CalculateStitchWire(model, model.PrintingCostEstimationId);
                            break;
                        default:
                            break;
                    }
                }
                if (model.Quantity == -1)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong! missing data on setting");
                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
                model.UnitCost = item.UnitPrice;
                model.UnitOfMeasurment = item.UnitOfMeasurement;
                model.TotalCost = model.UnitCost * model.Quantity;

                model.Quantity = model.Quantity;
                model.UnitCost = model.UnitCost;
                model.UnitOfMeasurment = model.UnitOfMeasurment;
                model.TotalCost = model.TotalCost;

                db.PrintingEstimationMaterialCosts.Attach(model);
                db.Entry(model).State = EntityState.Modified;
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
        private void DeleteRecord(PrintingEstimationMaterialCost model)
        {
            var entity = db.PrintingEstimationMaterialCosts.Find(model.PrintingEstimationMaterialCostId);
            db.PrintingEstimationMaterialCosts.Remove(entity);
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
