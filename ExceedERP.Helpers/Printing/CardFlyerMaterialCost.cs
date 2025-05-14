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

                var result = Math.Ceiling((paperQty * categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000);
                return Math.Ceiling(result * (1 + model.WastageInPercentage));
            }
            return -1;
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
            
            var inkQuantity = (double)materialCost.Quantity / 10000 * estimation.TextNoOfColors;
            return inkQuantity * (1 + model.WastageInPercentage);
        }

        public static double CalculatePlate(PrintingEstimationMaterialCost model, int printingCostEstimationId)
        {
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == printingCostEstimationId);
            if (estimation == null)
            {
                return -1;
            }
            var machine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId);
            if (machine == null)
            {
                return -1;
            }
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machine.PrintingMachineTypeId && x.PaperSizeId == estimation.CoverPaperSizeId);
            if (machineDetail == null)
            {
                return -1;
            }
            var plateQuantity = (estimation.CoverNoOfColor) * Math.Ceiling(estimation.ProductQuantity / machine.OutputRate * machineDetail.SectionUp); // Rounds to the next whole number
            return Math.Ceiling(plateQuantity * (1 + model.WastageInPercentage));
        }
    }
}
