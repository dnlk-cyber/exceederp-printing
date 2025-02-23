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
    public class PrintingEstimationLaborCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingEstimationLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int estId)
        {
            IQueryable<PrintingEstimationLaborCost> printingestimationlaborcosts = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estId);
            DataSourceResult result = printingestimationlaborcosts.ToDataSourceResult(request, printingEstimationLaborCost => new PrintingEstimationLaborCost
            {
                PrintingEstimationLaborCostId = printingEstimationLaborCost.PrintingEstimationLaborCostId,
                ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                LaborRate = printingEstimationLaborCost.LaborRate,
                TotalCost = printingEstimationLaborCost.TotalCost,
                DateCreated = printingEstimationLaborCost.DateCreated,
                LastModified = printingEstimationLaborCost.LastModified,
                CreatedBy = printingEstimationLaborCost.CreatedBy,
                ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                PrintingCostEstimationId = printingEstimationLaborCost.PrintingCostEstimationId,
                PrintingMachineTypeId = printingEstimationLaborCost.PrintingMachineTypeId,
                EstProductionPerHour = printingEstimationLaborCost.EstProductionPerHour,
                Quantity = printingEstimationLaborCost.Quantity,
                PrintingProcessName = printingEstimationLaborCost.PrintingProcessName
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationLaborCosts_Create([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost, int estId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationLaborCost
                {
                    ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                    PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                    EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                    LaborRate = printingEstimationLaborCost.LaborRate,
                    TotalCost = printingEstimationLaborCost.TotalCost,
                    DateCreated = DateTime.Today,
                    LastModified = printingEstimationLaborCost.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                    PrintingCostEstimationId = estId,
                    PrintingMachineTypeId = printingEstimationLaborCost.PrintingMachineTypeId,

                };
                entity.TotalCost = entity.LaborRate * entity.EstimatedHours;
                db.PrintingEstimationLaborCosts.Add(entity);
                db.SaveChanges();
                printingEstimationLaborCost.TotalCost = entity.TotalCost;
                printingEstimationLaborCost.PrintingEstimationLaborCostId = entity.PrintingEstimationLaborCostId;
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationLaborCosts_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationLaborCost
                {
                    PrintingEstimationLaborCostId = printingEstimationLaborCost.PrintingEstimationLaborCostId,
                    ProcessCategory = printingEstimationLaborCost.ProcessCategory,
                    PrintingProcessId = printingEstimationLaborCost.PrintingProcessId,
                    EstimatedHours = printingEstimationLaborCost.EstimatedHours,
                    LaborRate = printingEstimationLaborCost.LaborRate,
                    TotalCost = printingEstimationLaborCost.TotalCost,
                    DateCreated = printingEstimationLaborCost.DateCreated,
                    LastModified = printingEstimationLaborCost.LastModified,
                    CreatedBy = printingEstimationLaborCost.CreatedBy,
                    ModifiedBy = printingEstimationLaborCost.ModifiedBy,
                    PrintingCostEstimationId = printingEstimationLaborCost.PrintingCostEstimationId,
                    PrintingMachineTypeId = printingEstimationLaborCost.PrintingMachineTypeId
                };
                entity.TotalCost = entity.LaborRate * entity.EstimatedHours;

                db.PrintingEstimationLaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PrintingEstimationLaborCosts_Delete([DataSourceRequest]DataSourceRequest request, PrintingEstimationLaborCost printingEstimationLaborCost)
        {
            if (ModelState.IsValid)
            {
                var laborCost = db.PrintingEstimationLaborCosts.Find(printingEstimationLaborCost.PrintingEstimationLaborCostId);

                db.PrintingEstimationLaborCosts.Remove(laborCost);
                db.SaveChanges();
            }

            return Json(new[] { printingEstimationLaborCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult CalculateLaborCost(int estimationId)
        {
            /* 
            PlateMaking,
            PaperCutting,
            CoverPrinting,
            TextPrinting,
            CoverPlateChange,
            TextPlateChange,
            CoverColorChange,
            TextColorChange,
            Folding_FourPage_Sec,
            Collating_Manual_Gathering,
            Stitching_Manual_Side_Saddle, 
            Gathering,
            Thread_Sewing,
            GlueBinding,
            Trimming,
            Packing_Wrapping,
            Dispatching_Loading_Forklifting
            */
            var msg = new
            {
                success = "Successfully Created Labor Cost",
                error = "Failed! something went wrong!",
                Exist = "Labor cost exists"

            };
            var laborCost = db.PrintingEstimationLaborCosts.Where(x => x.PrintingCostEstimationId == estimationId);
            if (laborCost.Any())
            {
                return Json(msg.Exist, JsonRequestBehavior.AllowGet);
            }
            var costEstimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var jobType = db.PrintingJobTypes.FirstOrDefault(x => x.PrintingJobTypeId == costEstimation.JobTypeId);
            if (jobType == null)
            {
                return Json(msg.error, JsonRequestBehavior.AllowGet);
            }
            
            if (jobType.JobType == JobType.Book || jobType.JobType == JobType.Magazine)
            {
                CreatePlateMakingCost(estimationId);
                CreatePaperCuttingCost(estimationId);
                CreateCoverPrintingCost(estimationId);
                CreateTextPrintingCost(estimationId); // *
                CreateCoverPlateChangeCost(estimationId); // *
                CreateTextPlateChangeCost(estimationId);
                CreateCoverColorChange(estimationId); // *
                CreateTextColorChangeCost(estimationId); // *
                CreateFoldingCost(estimationId);
                CreateCollatingCost(estimationId);
                CreateStitchingCost(estimationId);
                CreateGatheringCost(estimationId);
                CreateThread_SewingCost(estimationId);
                CreateGlueBindingCost(estimationId);
                CreateTrimmingCost(estimationId);
                CreatePacking_WrappingCost(estimationId);
                CreateDispatching_Loading_ForkliftingCost(estimationId);
            }
            else if (jobType.JobType == JobType.Brochure || jobType.JobType == JobType.Card || jobType.JobType == JobType.Card)
            {
                CreatePlateMakingCost(estimationId);
                CreatePaperCuttingCost(estimationId);
                CreateCoverPrintingCost(estimationId);
                CreateTextPrintingCost(estimationId); // *
                CreateCoverPlateChangeCost(estimationId); // *
                CreateTextPlateChangeCost(estimationId);
                CreateCoverColorChange(estimationId); // *
                CreateTextColorChangeCost(estimationId); // *
                CreateFoldingCost(estimationId);
                CreateCollatingCost(estimationId);
                CreateStitchingCost(estimationId);
                CreateGatheringCost(estimationId);
                CreateThread_SewingCost(estimationId);
                CreateGlueBindingCost(estimationId);
                CreateTrimmingCost(estimationId);
                CreatePacking_WrappingCost(estimationId);
                CreateDispatching_Loading_ForkliftingCost(estimationId);
            }

            return Json(msg.success, JsonRequestBehavior.AllowGet);
        }

        #region Calculation Book
        
        private void CreatePlateMakingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                var categoryItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
                if(paperSize != null && categoryItem != null)
                {
                    var plateQuantity = (costEstimation.TotalTextNoPages * costEstimation.TextNoOfColors) / paperSize.NoOfPagesToA1Ratio;
                    var textPlateQuantity = plateQuantity * (1 + materialCost.WastageInPercentage);

                    var plateQuantityC = (costEstimation.CoverNoOfColor) / paperSize.NoOfPagesToA1Ratio;
                    var coverPlateQuantity = plateQuantityC * (1 + materialCost.WastageInPercentage);
                    var totalQuantity = textPlateQuantity + coverPlateQuantity;
                    CommonLaborCostSave(estimationId, PrintingProcessName.PlateMaking, PrintingProcessCategory.Pre_Printing, totalQuantity, true);
                    
                }               
            }
            
        }
        
        private void CreatePaperCuttingCost(int estimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);

            if (estimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.PaperSizeId);

                var paperQty = (estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio;
                var totalQuantity = paperQty * 2 * (1 + materialCost.WastageInPercentage);
                CommonLaborCostSave(estimationId, PrintingProcessName.PaperCutting, PrintingProcessCategory.Pre_Printing, totalQuantity, true);
            }
            
        }
        private void CreateCoverPrintingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var coverPrinting = (costEstimation.ProductQuantity / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;
                    CommonLaborCostSave(estimationId, PrintingProcessName.CoverPrinting, PrintingProcessCategory.Printing, coverPrinting, true);
                }
            }
        }
        private void CreateCoverPlateChangeCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var totalQuantity = (costEstimation.CoverNoOfColor / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage);
                    CommonLaborCostSave(estimationId, PrintingProcessName.CoverPlateChange, PrintingProcessCategory.Printing, totalQuantity, false);
                }
            }
        }
        private void CreateCoverColorChange(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var totalQuantity = (costEstimation.CoverNoOfColor / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage);
                    CommonLaborCostSave(estimationId, PrintingProcessName.CoverColorChange, PrintingProcessCategory.Printing, totalQuantity, false);
                }
            }
        }
        private void CreateTextPrintingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
               .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var textPrinting = ((costEstimation.ProductQuantity * costEstimation.TotalTextNoPages) / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;
                    CommonLaborCostSave(estimationId, PrintingProcessName.TextPrinting, PrintingProcessCategory.Printing, textPrinting, true);
                }
            }
        }
        private void CreateTextPlateChangeCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
              .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var totalQuantity = ((costEstimation.TotalTextNoPages * costEstimation.TextNoOfColors) / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage);
                    CommonLaborCostSave(estimationId, PrintingProcessName.TextPlateChange, PrintingProcessCategory.Printing, totalQuantity, false);
                }
            }
        }
        private void CreateTextColorChangeCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
              .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
            if (costEstimation != null && materialCost != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.PaperSizeId);
                if (paperSize != null)
                {
                    var totalQuantity = ((costEstimation.TotalTextNoPages * costEstimation.TextNoOfColors) / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage);
                    CommonLaborCostSave(estimationId, PrintingProcessName.TextColorChange, PrintingProcessCategory.Printing, totalQuantity, false);
                }
            }
        }

        #region Finishing
        private void CreateFoldingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
             .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Folding_FourPage_Sec, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateStitchingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
             .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Stitching_Manual_Side_Saddle, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateGatheringCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
            .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Gathering, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateCollatingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                         .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Collating_Manual_Gatering, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);
        }
        private void CreateThread_SewingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                        .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Thread_Sewing, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateGlueBindingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                       .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.GlueBinding, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateTrimmingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                       .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Trimming, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreatePacking_WrappingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                       .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Packing_Wrapping, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);

        }
        private void CreateDispatching_Loading_ForkliftingCost(int estimationId)
        {
            var costEstimation = db.PrintingCostEstimations
                       .FirstOrDefault(x => x.PrintingCostEstimationId == estimationId);
            CommonLaborCostSave(estimationId, PrintingProcessName.Dispatching_Loading_Forklifting, PrintingProcessCategory.Finishing, costEstimation.ProductQuantity, true);
        }
        #endregion
        private void CommonLaborCostSave(int id, PrintingProcessName printingProcessName, PrintingProcessCategory printingProcessCategory, double totalQuantity, bool isDivision)
        {
            var printingProcess = db.PrintingProcesses
                                            .FirstOrDefault(x => x.PrintingProcessName == printingProcessName);
            if (printingProcess != null)
            {
                decimal chargeablePerHour = 0;
                if (isDivision)
                {
                    chargeablePerHour = (decimal)totalQuantity / printingProcess.EstProductionPerHour;
                }
                else
                {
                    chargeablePerHour = (decimal)totalQuantity * printingProcess.EstProductionPerHour;
                }
                var totalCost = chargeablePerHour * printingProcess.LaborRate;
                var entity = new PrintingEstimationLaborCost
                {
                    Quantity = (decimal)totalQuantity,
                    EstProductionPerHour = printingProcess.EstProductionPerHour,
                    EstimatedHours = chargeablePerHour,
                    LaborRate = printingProcess.LaborRate,
                    ProcessCategory = printingProcessCategory,
                    PrintingProcessName = printingProcessName,
                    TotalCost = totalCost,
                    PrintingCostEstimationId = id
                };
                db.PrintingEstimationLaborCosts.Add(entity);
                db.SaveChanges();

            }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
