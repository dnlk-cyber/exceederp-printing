using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class VoucherMaterialCost
    {

        public static double CalculateTextPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
                if (paperSize == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Paper size not found for ID: {estimation.TextPaperSizeId}");
                    return -1;
                }

                var categoryItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == model.PrintingMaterialCategoryItemId);
                if (categoryItem == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Category item not found for ID: {model.PrintingMaterialCategoryItemId}");
                    return -1;
                }

                var paperQty = Math.Ceiling((double)(estimation.ProductQuantity * estimation.Pages * estimation.Copy) / paperSize.NoOfPagesToA1Ratio);
                if (categoryItem.PaperType == PaperType.SheetFed)
                {
                    return Math.Ceiling(paperQty * (1 + model.WastageInPercentage));
                }

                var result = Math.Ceiling((paperQty * categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000);
                return Math.Ceiling(result * (1 + model.WastageInPercentage));
            }

        }

        public static double CalculateCoverPaperQuantity(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.CoverPaperSizeId);
                if (paperSize == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Cover paper size not found for ID: {estimation.CoverPaperSizeId}");
                    return -1;
                }

                var paperQty = Math.Ceiling((double)(estimation.ProductQuantity) / paperSize.NoOfPagesToA1Ratio);
                return Math.Ceiling(paperQty * (1 + model.WastageInPercentage));
            }
            
        }

        public static double CalculateTextPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var machine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId);
                if (machine == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Text machine not found for ID: {estimation.TextMachineTypeId}");
                    return -1;
                }

                var machineDetail = db.PrintingMachineDetails
                    .FirstOrDefault(x => x.PrintingMachineTypeId == machine.PrintingMachineTypeId && x.PaperSizeId == estimation.TextPaperSizeId);
                if (machineDetail == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Machine detail not found for MachineTypeId: {machine.PrintingMachineTypeId}, PaperSizeId: {estimation.TextPaperSizeId}");
                    return -1;
                }

                var plateQuantity = Math.Ceiling((double)(estimation.TextNoOfColors * estimation.TextPrintSide) * Math.Ceiling(estimation.ProductQuantity / machine.OutputRate));
                return Math.Ceiling(plateQuantity * (1 + model.WastageInPercentage));
            }
            
        }
        public static double CalculateCoverPlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var machine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId);
                if (machine == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Cover machine not found for ID: {estimation.CoverMachineTypeId}");
                    return -1;
                }

                var machineDetail = db.PrintingMachineDetails
                    .FirstOrDefault(x => x.PrintingMachineTypeId == machine.PrintingMachineTypeId && x.PaperSizeId == estimation.CoverPaperSizeId);
                if (machineDetail == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Machine detail not found for MachineTypeId: {machine.PrintingMachineTypeId}, PaperSizeId: {estimation.CoverPaperSizeId}");
                    return -1;
                }
                var plateQuantity = Math.Ceiling((double)(estimation.CoverNoOfColor * estimation.CoverPrintSide) * Math.Ceiling(estimation.ProductQuantity / machine.OutputRate));
                return Math.Ceiling((double)plateQuantity * (1 + model.WastageInPercentage));
            }
            
        }

        public static double CalculatePackagingPaper(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var packs = estimation.ProductQuantity / estimation.Pack;
                return (double)packs / 2;
            }
           
        }
        public static double CalculateGlue(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                double glueQuantity = 0;
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var paperSize = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId);
                if (paperSize == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Paper size not found for ID: {estimation.TextPaperSizeId}");
                    return -1;
                }

                if (estimation.UseLength)
                {
                    glueQuantity = estimation.ProductQuantity / (1 / (paperSize.Length / 100 * 0.015));
                    return glueQuantity * (1 + model.WastageInPercentage);
                }
                glueQuantity = estimation.ProductQuantity / (1 / (paperSize.Width / 100 * 0.015));
                return glueQuantity * (1 + model.WastageInPercentage);
            }
           
        }
        public static double CalculateStitchWire(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var item = db.PrintingMaterialCategoryItems
                    .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == model.PrintingMaterialCategoryItemId);
                if (item == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Material category item not found for ID: {model.PrintingMaterialCategoryItemId}");
                    return -1;
                }

                var stitchWire = estimation.ProductQuantity * estimation.Staples / item.ConsumptionRate;
                return stitchWire * (1 + model.WastageInPercentage);
            }
            
        }

        public static double CalculateInkText(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
                if (estimation == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Estimation not found for ID: {printingCostEstimationId}");
                    return -1;
                }

                var materialCost = db.PrintingEstimationMaterialCosts
                    .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                    && x.EstimationMaterialType == EstimationMaterialType.TextPaper);
                if (materialCost == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Text paper material cost not found for estimation ID: {estimation.PrintingCostEstimationId}");
                    return -1;
                }

                var item = db.PrintingMaterialCategoryItems
                    .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
                if (item == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Material category item not found for ID: {materialCost.PrintingMaterialCategoryItemId}");
                    return -1;
                }

                int constant;
                if (item.Weight >= 80)
                    constant = 23;
                else
                    constant = 31;

                var inkQuantity = (double)((materialCost.Quantity * constant) / (10000 * estimation.TextPrintSide)) * estimation.TextNoOfColors;
                return inkQuantity * (1 + model.WastageInPercentage);
            }
            
        }
        public static double CalculateInkCover(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            using (var db = new ExceedDbContext())
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
                return (double)(inkQuantity * (1 + model.WastageInPercentage));
            }
            
        }

    }
}
