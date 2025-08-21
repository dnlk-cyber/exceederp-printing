using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class CardFlyerMaterialCost
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

                var paperQty = Math.Ceiling((double)estimation.ProductQuantity * estimation.Pages / (paperSize.NoOfPagesToA1Ratio * 2));
                if (categoryItem.PaperType == PaperType.SheetFed)
                {
                    return Math.Ceiling(paperQty * (1 + model.WastageInPercentage));
                }

                var result = Math.Ceiling((paperQty * categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000);
                return Math.Ceiling(result * (1 + model.WastageInPercentage));
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

                var inkQuantity = (double)materialCost.Quantity / 10000 * estimation.TextNoOfColors;
                return inkQuantity * (1 + model.WastageInPercentage);
            }
        }

        public static double CalculatePlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
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

                var plateQuantity = (estimation.CoverNoOfColor) * Math.Ceiling(estimation.ProductQuantity / machine.OutputRate * machineDetail.SectionUp);
                return Math.Ceiling((double)plateQuantity * (1 + model.WastageInPercentage));
            }
        }
    }
}