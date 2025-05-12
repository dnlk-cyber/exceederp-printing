using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class MaterialWeight
    {
        readonly static private ExceedDbContext db = new ExceedDbContext();

        public static double TotalProductWeight(PrintingCostEstimation estimation)
        {
            
            return TextPaperWeight(estimation) + CoverPaperWeight(estimation) + PackingPaperWeight(estimation) 
                + InkWeight(estimation) + GlueWeight(estimation);
        }

        public static double UnitProductWeight(PrintingCostEstimation estimation)
        {
            return TotalProductWeight(estimation) / estimation.ProductQuantity;
        }

        public static double PackageWeight(PrintingCostEstimation estimation)
        {
            return UnitProductWeight(estimation) * (double)estimation.Pack;
        }
        public static double TextPaperWeight(PrintingCostEstimation estimation)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.TextPaper); // text paper
            if (materialCost == null)
                return 0;

            var paper = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == estimation.TextPaperSizeId); // text paper
            if (paper == null)
                return 0;
            var categoryItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
            if (categoryItem == null)
                return 0;
            return ((paper.Length * paper.Width * categoryItem.Weight) / 10000000)
                * estimation.ProductQuantity * estimation.Pages / estimation.TextPrintSide;

        }

        public static double CoverPaperWeight(PrintingCostEstimation estimation)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.CoverPaper); // cover paper
            if (materialCost == null)
                return 0;
            var paper = db.PrintingPaperSizes
                .FirstOrDefault(x => x.PrintingPaperSizeId == estimation.CoverPaperSizeId); // cover paper
            if (paper == null)
                return 0;
            var categoryItem = db.PrintingMaterialCategoryItems
                .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
            if (categoryItem == null)
                return 0;
            return ((paper.Length * paper.Width * categoryItem.Weight) / 10000000) * estimation.ProductQuantity;

        }

        public static double PackingPaperWeight(PrintingCostEstimation estimation)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.WrappingPaper); // packing paper

            if (materialCost == null)
                return 0;

            var categoryItem = db.PrintingMaterialCategoryItems
                .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);
            if (categoryItem == null)
                return 0;
            return ((categoryItem.Length * categoryItem.Width * categoryItem.Weight) / 10000000) * (double)materialCost.Quantity;

        }

        public static double InkWeight(PrintingCostEstimation estimation)
        {
            var inkText = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.InkText)?.Quantity ?? 0; // ink text
            var inkCover = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.InkCover)?.Quantity ?? 0; // ink cover
            return (double)(inkText + inkCover) * 0.6;

        }

        public static double GlueWeight(PrintingCostEstimation estimation)
        {
            var glueQuantity = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId
                && x.EstimationMaterialType == EstimationMaterialType.Glue)?.Quantity ?? 0; // ink text

            return (double)(glueQuantity) * 0.6;

        }
    }
}
