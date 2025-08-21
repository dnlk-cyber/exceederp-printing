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
            model.WastageInPercentage = model.WastageInPercentage / 100;

            using (var db = new ExceedDbContext())
            {
                var existingMaterialCost = db.PrintingEstimationMaterialCosts.SingleOrDefault(x => x.PrintingCostEstimationId == estId && x.EstimationMaterialType == model.EstimationMaterialType);
                if (existingMaterialCost != null)
                    ModelState.AddModelError("", $"{existingMaterialCost.EstimationMaterialType.ToString()} Exists!");

                if (ModelState.IsValid)
                {
                    var item = db.PrintingMaterialCategoryItems.Find(model.PrintingMaterialCategoryItemId);
                    if (item == null)
                    {
                        ModelState.AddModelError(string.Empty, "Material category item not found");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

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
                    if (costEstimation == null)
                    {
                        ModelState.AddModelError(string.Empty, "Cost estimation not found");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    double calculatedQuantity = 0;

                    if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine || costEstimation.PrintJobType == PrintJobType.Newsletter)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateTextPaperQuantity(model, estId);
                                break;
                            case EstimationMaterialType.CoverPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateCoverPaperQuantity(model, estId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateTextPlate(model, estId);
                                break;
                            case EstimationMaterialType.CoverPlate:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateCoverPlate(model, estId);
                                break;
                            case EstimationMaterialType.Glue:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateGlue(model, estId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateInkText(model, estId);
                                break;
                            case EstimationMaterialType.InkCover:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateInkCover(model, estId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculatePackagingPaper(model, estId);
                                break;
                            case EstimationMaterialType.StitchingWire:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateStitchWire(model, estId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Flyer ||
                        costEstimation.PrintJobType == PrintJobType.Format || costEstimation.PrintJobType == PrintJobType.Poster || costEstimation.PrintJobType == PrintJobType.Sticker)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = CardFlyerMaterialCost.CalculateTextPaperQuantity(model, estId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = CardFlyerMaterialCost.CalculatePlate(model, estId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = CardFlyerMaterialCost.CalculateInkText(model, estId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = CardFlyerMaterialCost.CalculatePackagingPaper(model, estId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else if (costEstimation.PrintJobType == PrintJobType.Voucher)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculateTextPaperQuantity(model, estId);
                                break;
                            case EstimationMaterialType.CoverPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculateCoverPaperQuantity(model, estId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = VoucherMaterialCost.CalculateTextPlate(model, estId);
                                break;
                            case EstimationMaterialType.CoverPlate:
                                calculatedQuantity = VoucherMaterialCost.CalculateCoverPlate(model, estId);
                                break;
                            case EstimationMaterialType.Glue:
                                calculatedQuantity = VoucherMaterialCost.CalculateGlue(model, estId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = VoucherMaterialCost.CalculateInkText(model, estId);
                                break;
                            case EstimationMaterialType.InkCover:
                                calculatedQuantity = VoucherMaterialCost.CalculateInkCover(model, estId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculatePackagingPaper(model, estId);
                                break;
                            case EstimationMaterialType.StitchingWire:
                                calculatedQuantity = VoucherMaterialCost.CalculateStitchWire(model, estId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Unsupported print job type");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    if (calculatedQuantity == -1)
                    {
                        ModelState.AddModelError(string.Empty, "Calculation failed - missing data or invalid configuration. Check the debug output for details.");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    entity.Quantity = (decimal)calculatedQuantity;
                    entity.UnitCost = item.UnitPrice;
                    entity.UnitOfMeasurment = item.UnitOfMeasurement;
                    entity.TotalCost = entity.UnitCost * entity.Quantity;

                    model.Quantity = entity.Quantity;
                    model.UnitCost = entity.UnitCost;
                    model.UnitOfMeasurment = entity.UnitOfMeasurment;
                    model.TotalCost = entity.TotalCost;

                    try
                    {
                        db.PrintingEstimationMaterialCosts.Add(entity);
                        db.SaveChanges();
                        model.PrintingEstimationMaterialCostId = entity.PrintingEstimationMaterialCostId;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to save: {ex.Message}");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationMaterialCost model)
        {
            model.WastageInPercentage = model.WastageInPercentage / 100;

            using (var db = new ExceedDbContext())
            {
                var existingMaterialCost = db.PrintingEstimationMaterialCosts
                    .SingleOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId
                    && x.EstimationMaterialType == model.EstimationMaterialType
                    && x.PrintingEstimationMaterialCostId != model.PrintingEstimationMaterialCostId);
                if (existingMaterialCost != null)
                    ModelState.AddModelError("", $"{existingMaterialCost.EstimationMaterialType.ToString()} Exists!");

                if (ModelState.IsValid)
                {
                    var item = db.PrintingMaterialCategoryItems.Find(model.PrintingMaterialCategoryItemId);
                    if (item == null)
                    {
                        ModelState.AddModelError(string.Empty, "Material category item not found");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == model.PrintingCostEstimationId);
                    if (costEstimation == null)
                    {
                        ModelState.AddModelError(string.Empty, "Cost estimation not found");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    double calculatedQuantity = 0;

                    if (costEstimation.PrintJobType == PrintJobType.Book || costEstimation.PrintJobType == PrintJobType.Magazine || costEstimation.PrintJobType == PrintJobType.Newsletter)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.CoverPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateTextPlate(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.CoverPlate:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateCoverPlate(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.Glue:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateGlue(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateInkText(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.InkCover:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateInkCover(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.StitchingWire:
                                calculatedQuantity = BookMagazineMaterialCostEstimation.CalculateStitchWire(model, model.PrintingCostEstimationId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else if (costEstimation.PrintJobType == PrintJobType.Brochure || costEstimation.PrintJobType == PrintJobType.Card || costEstimation.PrintJobType == PrintJobType.Flyer ||
                        costEstimation.PrintJobType == PrintJobType.Format || costEstimation.PrintJobType == PrintJobType.Poster || costEstimation.PrintJobType == PrintJobType.Sticker)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = CardFlyerMaterialCost.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = CardFlyerMaterialCost.CalculatePlate(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = CardFlyerMaterialCost.CalculateInkText(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = CardFlyerMaterialCost.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else if (costEstimation.PrintJobType == PrintJobType.Voucher)
                    {
                        switch (model.EstimationMaterialType)
                        {
                            case EstimationMaterialType.TextPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.CoverPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.TextPlate:
                                calculatedQuantity = VoucherMaterialCost.CalculateTextPlate(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.CoverPlate:
                                calculatedQuantity = VoucherMaterialCost.CalculateCoverPlate(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.Glue:
                                calculatedQuantity = VoucherMaterialCost.CalculateGlue(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.InkText:
                                calculatedQuantity = VoucherMaterialCost.CalculateInkText(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.InkCover:
                                calculatedQuantity = VoucherMaterialCost.CalculateInkCover(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.WrappingPaper:
                                calculatedQuantity = VoucherMaterialCost.CalculatePackagingPaper(model, model.PrintingCostEstimationId);
                                break;
                            case EstimationMaterialType.StitchingWire:
                                calculatedQuantity = VoucherMaterialCost.CalculateStitchWire(model, model.PrintingCostEstimationId);
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Unsupported material type for this job type");
                                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Unsupported print job type");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    if (calculatedQuantity == -1)
                    {
                        ModelState.AddModelError(string.Empty, "Calculation failed - missing data or invalid configuration. Check the debug output for details.");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }

                    model.Quantity = (decimal)calculatedQuantity;
                    model.UnitCost = item.UnitPrice;
                    model.UnitOfMeasurment = item.UnitOfMeasurement;
                    model.TotalCost = model.UnitCost * model.Quantity;

                    try
                    {
                        db.PrintingEstimationMaterialCosts.Attach(model);
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to save: {ex.Message}");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }
                }
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
