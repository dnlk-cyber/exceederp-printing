using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class BookMagazineLaborCostEstimation
    {
        private static readonly ExceedDbContext db = new ExceedDbContext();

        // Labour activities here
        #region PrePress
        public static void DesignCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.Pages + 4,

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CompositionCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.Pages + 4,

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void TextPlateMakingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                x.EstimationMaterialType == EstimationMaterialType.TextPlate);

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = materialCost.Quantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CoverPlateMakingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                x.EstimationMaterialType == EstimationMaterialType.CoverPlate);

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = materialCost.Quantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void ProofingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.Pages,

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        #endregion

        #region Press
        public static void TextMakeReadyCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                x.EstimationMaterialType == EstimationMaterialType.TextPlate);

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = materialCost.Quantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CoverMakeReadyCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
                .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                x.EstimationMaterialType == EstimationMaterialType.CoverPlate);

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = materialCost.Quantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void TextPrintCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId); // text machine
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.TextPaperSizeId); // text paper
                
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                
            };
            entity.Quantity = (estimation.ProductQuantity * estimation.Pages * estimation.TextNoOfColors) /
                (machineDetail.SectionUp * machineType.NumberOfColors * estimation.TextPrintSide);
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CoverPrintCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
           
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId); // cover machine
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.CoverPaperSizeId); // cover paper

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,

            };
            entity.Quantity = (estimation.ProductQuantity * estimation.CoverNoOfColor) /
                (machineDetail.SectionUp * machineType.NumberOfColors);
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        #endregion

        #region PostPress
        public static void CuttingCoverCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            var materialCost = db.PrintingEstimationMaterialCosts
               .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
               x.EstimationMaterialType == EstimationMaterialType.CoverPaper); // cover paper

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = materialCost.Quantity,

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void FoldingCoverCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
           
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.ProductQuantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CollatingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = PrintSignitureAndSheet.PrintSigniture(estimation)

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void PerfectBindingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.ProductQuantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void TrimmingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.ProductQuantity

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void CheckingPackingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {

            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                Quantity = estimation.ProductQuantity/estimation.Pack

            };
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void LoadingUnloadingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {

            
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,
                
            };
            entity.Quantity = (decimal)MaterialWeight.TotalProductWeight(estimation);
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        public static void DispatchingTransportingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            
            var entity = new PrintingEstimationLaborCost
            {
                DateCreated = DateTime.Today,
                LabourActivity = printingProcess.LabourActivity,
                PressCategory = printingProcess.PressCategory,
                PrintingCostEstimationId = estimation.PrintingCostEstimationId,
                LaborRate = printingProcess.LaborRate,
                UOM = printingProcess.UOM,
                PrintingProcessId = printingProcess.PrintingProcessId,

            };
            entity.Quantity = (decimal)MaterialWeight.TotalProductWeight(estimation);
            entity.TotalCost = entity.Quantity * entity.LaborRate;
            db.PrintingEstimationLaborCosts.Add(entity);
            db.SaveChanges();

        }
        #endregion



    }
}
