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
                var paperQty = Math.Ceiling((double)estimation.ProductQuantity * estimation.Pages / (paperSize.NoOfPagesToA1Ratio * 2));
                if (categoryItem.PaperType == PaperType.SheetFed)
                {
                    return Math.Ceiling(paperQty * (1 + model.WastageInPercentage));
                }
               
                var result = (paperQty * categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000;
                return Math.Ceiling(result * (1 + model.WastageInPercentage));
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

            var paperQty = Math.Ceiling((double)estimation.ProductQuantity / paperSize.NoOfPagesToA1Ratio);
            
            return Math.Ceiling(paperQty * (1 + model.WastageInPercentage));
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
            var plateQuantity = Math.Ceiling(((estimation.Pages * estimation.TextNoOfColors)/machineDetail.SectionUp) * Math.Ceiling(estimation.ProductQuantity / machine.OutputRate)); // Rounds to the next whole number
            return Math.Ceiling(plateQuantity * (1 + model.WastageInPercentage));
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
            var plateQuantity = (estimation.CoverNoOfColor) * Math.Ceiling(estimation.ProductQuantity / (machine.OutputRate * machineDetail.SectionUp)); // Rounds to the next whole number
            return Math.Ceiling(plateQuantity * (1 + model.WastageInPercentage));
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
            return ((double)packs / 2) * (1 + model.WastageInPercentage);
        }
        public static double CalculateGlue(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            double glueQuantity = 0;
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if(estimation == null)
            {
                return -1;
            }
            var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
            //Glue = Quantity/(1/paperlength/100*1.5/100)
            if (estimation.UseLength)
            {
                glueQuantity = estimation.ProductQuantity / (1 / (paperSize.Length / 100 * 0.015));

                return glueQuantity * (1 + model.WastageInPercentage);
            }
            glueQuantity = estimation.ProductQuantity / (1 / (paperSize.Width / 100 * 0.015));
            return Math.Ceiling(glueQuantity * (1 + model.WastageInPercentage));
        }
        public static double CalculateStitchWire(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {

            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var item = db.PrintingMaterialCategoryItems
                .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == model.PrintingMaterialCategoryItemId);
            if (item == null)
                return -1;
            var stitchWire = estimation.ProductQuantity * estimation.Staples / item.ConsumptionRate;
            return Math.Ceiling(stitchWire * (1 + model.WastageInPercentage));
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
            if (materialCost == null)
                return -1;
            var item = db.PrintingMaterialCategoryItems
                .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
            int constant;
            if (item.Weight >= 80)
                constant = 23;
            else
                constant = 31;
            var inkQuantity = (double)((materialCost.Quantity * constant) / (10000 * estimation.TextPrintSide)) * estimation.TextNoOfColors;
            return inkQuantity * (1 + model.WastageInPercentage);
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
            var item = db.PrintingMaterialCategoryItems
                .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
            int constant;
            if (item.Weight <= 80)
                constant = 10000;
            else if (item.Weight >= 100 && item.Weight <= 150)
                constant = 9000;
            else
                constant = 8000;
            var inkQuantity = (double)(materialCost.Quantity / constant) * estimation.CoverNoOfColor;
            return inkQuantity * (1 + model.WastageInPercentage);
        }
       

    }
}
