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
                PrintingCostEstimationId = printingEstimationMaterialCost.PrintingCostEstimationId,
                EstimationMaterialType = printingEstimationMaterialCost.EstimationMaterialType,
                PaperWeight = printingEstimationMaterialCost.PaperWeight,
                WastageInPercentage = printingEstimationMaterialCost.WastageInPercentage,
                
            });

            return Json(result);
        }

        private double CalculateTextPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);
                var categoryItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == model.PrintingMaterialCategoryItemId);
                var paperQty = (estimation.ProductQuantity * estimation.TotalTextNoPages) / (paperSize.NoOfPagesToA1Ratio * 2);

                var paperQtyWithWastage = paperQty * (1 + model.WastageInPercentage);

                return (paperQtyWithWastage * paperSize.Length * paperSize.Width * categoryItem.Weight) / 10000000;
            }
            return 0;
        }
        private double CalculateCoverPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(estimation == null)
            {
                return 0;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

            var paperQty = (estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio;
            return paperQty * 2 * (1 + model.WastageInPercentage);
        }
        private double CalculateTextPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(estimation == null)
            {
                return 0;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

            var plateQuantity = (estimation.TotalTextNoPages * estimation.TextNoOfColors) / paperSize.NoOfPagesToA1Ratio;
            return plateQuantity * (1 + model.WastageInPercentage);
        }
        private double CalculateCoverPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(estimation == null)
            {
                return 0;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

            var plateQuantity = (estimation.CoverNoOfColor) / paperSize.NoOfPagesToA1Ratio;
            return plateQuantity * (1 + model.WastageInPercentage);
        }
        #region Card, Brochure, Flyer
        private double CalculateTextPaperQuantityCard(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return 0;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

            var paperQty = (estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio;
            return paperQty * (1 + model.WastageInPercentage);
        }
        private double CalculateTextPlateCard(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return 0;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

            var plateQuantity = (estimation.CoverNoOfColor) / paperSize.NoOfPagesToA1Ratio;
            return plateQuantity * (1 + model.WastageInPercentage);
        }
        #endregion  

        private double CalculateInk(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var textPrinting = ((costEstimation.ProductQuantity * costEstimation.TotalTextNoPages) / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;
                    return textPrinting / 7500; 
                }
            }
            return 0;
        }
        private double CalculateCoverPaper(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    return (costEstimation.ProductQuantity / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;
                    
                }
            }
            return 0;
        }
        private double CalculateInk100Cover(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            return CalculateCoverPaper(model, printingCostEstimationId) / 1200;
        }
        private double CalculateInkOther(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            return (CalculateCoverPaper(model, printingCostEstimationId) / 1200) * 3;
        }
        private double CalculateStitchingWire(int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(costEstimation != null)
            {
                return costEstimation.ProductQuantity / (12500 * 96 / costEstimation.TotalTextNoPages);
            }
            return 0;
        }
        private double CalculateSewingThread(int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (costEstimation != null)
            {
                return costEstimation.ProductQuantity / 100;
            }
            return 0;
        }
        private double CalculateHotMeltGlue(int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (costEstimation != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);

                var page = costEstimation.TotalTextNoPages;
                var length = paperSize.Length;
                return costEstimation.ProductQuantity / (296*180/page*29.7/length);
            }
            return 0;
        }
        private double CalculateWrappingPaper(int printingCostEstimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (costEstimation != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);

                var page = costEstimation.TotalTextNoPages;
                var length = paperSize.Length;
                return costEstimation.ProductQuantity / (30 * 132 / page * 29.7 /length);
            }
            return 0;
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
                var jobType = db.PrintingJobTypes.FirstOrDefault(x => x.PrintingJobTypeId == costEstimation.JobTypeId);
                if (jobType.JobType == JobType.Book || jobType.JobType == JobType.Magazine)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)CalculateTextPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            entity.Quantity = (decimal)CalculateCoverPaperQuantity(model, estId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)CalculateTextPlate(model, estId);
                            break;
                        case EstimationMaterialType.Ink:
                            entity.Quantity = (decimal)CalculateInk(model, estId);
                            break;
                        case EstimationMaterialType.Ink_100CoverageOnA4Cover:
                            entity.Quantity = (decimal)CalculateInk100Cover(model, estId);
                            break;
                        case EstimationMaterialType.Ink_Other:
                            entity.Quantity = (decimal)CalculateInkOther(model, estId);
                            break;
                        case EstimationMaterialType.StitchingWire:
                            entity.Quantity = (decimal)CalculateStitchingWire(estId);
                            break;
                        case EstimationMaterialType.Sewing_Thread:
                            entity.Quantity = (decimal)CalculateSewingThread(estId);
                            break;
                        case EstimationMaterialType.HotMelt_Glue:
                            entity.Quantity = (decimal)CalculateHotMeltGlue(estId);
                            break;
                        case EstimationMaterialType.Wrapping_Paper:
                            entity.Quantity = (decimal)CalculateWrappingPaper(estId);
                            break;
                        default:
                            entity.Quantity = (decimal)CalculateCoverPlate(model, estId);
                            break;
                    }
                }
                else if (jobType.JobType == JobType.Brochure || jobType.JobType == JobType.Card || jobType.JobType == JobType.Card)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)CalculateTextPaperQuantityCard(model, estId);
                            break;                       
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)CalculateTextPlateCard(model, estId);
                            break;                        
                    }
                }
                
                entity.UnitCost = item.UnitPrice;
                entity.UnitOfMeasurment = item.UnitOfMeasurement;
                entity.TotalCost = entity.UnitCost * entity.Quantity;

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
                var jobType = db.PrintingJobTypes.FirstOrDefault(x => x.PrintingJobTypeId == costEstimation.JobTypeId);
                if (jobType.JobType == JobType.Book || jobType.JobType == JobType.Magazine)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)CalculateTextPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.CoverPaper:
                            entity.Quantity = (decimal)CalculateCoverPaperQuantity(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)CalculateTextPlate(model, model.PrintingCostEstimationId);
                            break;
                        default:
                            entity.Quantity = (decimal)CalculateCoverPlate(model, model.PrintingCostEstimationId);
                            break;
                    }
                }
                else if (jobType.JobType == JobType.Brochure || jobType.JobType == JobType.Card || jobType.JobType == JobType.Card)
                {
                    switch (model.EstimationMaterialType)
                    {
                        case EstimationMaterialType.TextPaper:
                            entity.Quantity = (decimal)CalculateTextPaperQuantityCard(model, model.PrintingCostEstimationId);
                            break;
                        case EstimationMaterialType.TextPlate:
                            entity.Quantity = (decimal)CalculateTextPlateCard(model, model.PrintingCostEstimationId);
                            break;
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
