using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class BookMagazineMaterialCostEstimation
    {
        readonly static private ExceedDbContext db = new ExceedDbContext();

        public static double CalculateTextPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation != null)
            {
                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
                var categoryItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == model.PrintingMaterialCategoryItemId);
                var paperQty = (estimation.ProductQuantity * estimation.Pages) / (paperSize.NoOfPagesToA1Ratio * 2);
                if (categoryItem.PaperType == PaperType.SheetFed)
                {
                    return paperQty;
                }
               
                return (paperQty * categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000 * (1 + model.WastageInPercentage);
            }
            return -1;
        }
        public static double CalculateCoverPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.CoverPaperSizeId);

            var paperQty = (estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio;
            if(estimation.CoverPrintSide == 2m)
                return paperQty * 2 * (1 + model.WastageInPercentage);
            return paperQty * (1 + model.WastageInPercentage);
        }
        public static double CalculateTextPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var machine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId);
            if (machine == null)
            {
                return -1;
            }
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machine.PrintingMachineTypeId && x.PaperSizeId == estimation.TextPaperSizeId);
            if(machineDetail == null)
            {
                return -1;
            }
            
            var plateQuantity = ((estimation.Pages * estimation.TextNoOfColors)/machineDetail.SectionUp) * Math.Round((estimation.ProductQuantity / machine.OutputRate) + 0.5); // Rounds to the next whole number
            return plateQuantity * (1 + model.WastageInPercentage);
        }
        public static double CalculateCoverPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var machine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId);
            if(machine == null)
            {
                return -1;
            }
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machine.PrintingMachineTypeId && x.PaperSizeId == estimation.CoverPaperSizeId);
            if(machineDetail == null)
            {
                return -1;
            }
            
            var plateQuantity = (estimation.CoverNoOfColor / machineDetail.SectionUp) * Math.Round(estimation.ProductQuantity / machine.OutputRate + 0.5); // Rounds to the next whole number
            return plateQuantity * (1 + model.WastageInPercentage);
        }

        public static double CalculatePackagingPaper(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            //PackagingPaper = (Quantity/Input)/2
            var packs = estimation.ProductQuantity / estimation.Pack;
            return (double)packs / 2;
        }
        public static double CalculateGlue(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(estimation == null)
            {
                return -1;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
            //Glue = Quantity/(1/paperlength/100*1.5/100)
            if (estimation.UseLength)
            {
                return estimation.ProductQuantity / (1 / (paperSize.Length / 100 * 0.015));
            }
            return estimation.ProductQuantity / (1 / (paperSize.Width / 100 * 0.015));
        }

        public static double CalculateInkText(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.TextPaper);
            return (double)((materialCost.Quantity * 23) / (10000 * estimation.TextPrintSide)) * (estimation.TextNoOfColors * 1.05);
        }
        public static double CalculateInkCover(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.CoverPaper);
            return (double)(materialCost.Quantity / 8000) * estimation.CoverNoOfColor * 1.05;
        }
        //#region Card, Brochure, Flyer
        //public static double CalculateTextPaperQuantityCard(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (estimation == null)
        //    {
        //        return -1;
        //    }
        //    var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);

        //    var paperQty = (estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio;
        //    return paperQty * (1 + model.WastageInPercentage);
        //}
        //public static double CalculateTextPlateCard(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (estimation == null)
        //    {
        //        return -1;
        //    }
        //    var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);

        //    var plateQuantity = (estimation.CoverNoOfColor) / paperSize.NoOfPagesToA1Ratio;
        //    return plateQuantity * (1 + model.WastageInPercentage);
        //}
        //#endregion  

        //public static double CalculateInk(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //       .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
        //    if (costEstimation != null && materialCost != null)
        //    {
        //        var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.TextPaperSizeId);
        //        if (paperSize != null)
        //        {
        //            var textPrinting = ((costEstimation.ProductQuantity * costEstimation.Pages) / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;
        //            return textPrinting / 7500;
        //        }
        //    }
        //    return -1;
        //}
        //public static double CalculateCoverPaper(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //        .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    var materialCost = db.PrintingEstimationMaterialCosts.FirstOrDefault(x => x.PrintingCostEstimationId == costEstimation.PrintingCostEstimationId);
        //    if (costEstimation != null && materialCost != null)
        //    {
        //        var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.CoverPaperSizeId);
        //        if (paperSize != null)
        //        {
        //            return (costEstimation.ProductQuantity / paperSize.NoOfPagesToA1Ratio) * (1 + materialCost.WastageInPercentage) * costEstimation.TextNoOfColors;

        //        }
        //    }
        //    return -1;
        //}
        //public static double CalculateInk100Cover(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    return CalculateCoverPaper(model, printingCostEstimationId) / 1200;
        //}
        //public static double CalculateInkOther(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        //{
        //    return (CalculateCoverPaper(model, printingCostEstimationId) / 1200) * 3;
        //}
        //public static double CalculateStitchingWire(int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //       .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (costEstimation != null)
        //    {
        //        return costEstimation.ProductQuantity / (12500 * 96 / costEstimation.Pages);
        //    }
        //    return -1;
        //}
        //public static double CalculateSewingThread(int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //       .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (costEstimation != null)
        //    {
        //        return costEstimation.ProductQuantity / 100;
        //    }
        //    return -1;
        //}
        //public static double CalculateHotMeltGlue(int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //       .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (costEstimation != null)
        //    {
        //        var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.TextPaperSizeId);

        //        var page = costEstimation.Pages;
        //        var length = paperSize.Length;
        //        return costEstimation.ProductQuantity / (296 * 180 / page * 29.7 / length);
        //    }
        //    return -1;
        //}
        //public static double CalculateWrappingPaper(int printingCostEstimationId)
        //{
        //    var costEstimation = db.PrintingCostEstimations
        //       .FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
        //    if (costEstimation != null)
        //    {
        //        var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == costEstimation.TextPaperSizeId);

        //        var page = costEstimation.Pages;
        //        var length = paperSize.Length;
        //        return costEstimation.ProductQuantity / (30 * 132 / page * 29.7 / length);
        //    }
        //    return -1;
        //}

    }
}
