using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class CardFlyerLaborCost
    {
        // REMOVED: readonly static private ExceedDbContext db = new ExceedDbContext();

        #region PrePress
        public static void DesignCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }

        public static void CompositionCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }

        public static void PlateMakingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
            {
                var materialCost = db.PrintingEstimationMaterialCosts
                    .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                    x.EstimationMaterialType == EstimationMaterialType.TextPlate);

                if (materialCost != null)
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
                        Quantity = materialCost.Quantity
                    };
                    entity.TotalCost = entity.Quantity * entity.LaborRate;
                    db.PrintingEstimationLaborCosts.Add(entity);
                    db.SaveChanges();
                }
            }
        }

        public static void ProofingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }
        #endregion

        #region Press
        public static void TextMakeReadyCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
            {
                var materialCost = db.PrintingEstimationMaterialCosts
                    .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                    x.EstimationMaterialType == EstimationMaterialType.TextPlate);

                if (materialCost != null)
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
                        Quantity = materialCost.Quantity
                    };
                    entity.TotalCost = entity.Quantity * entity.LaborRate;
                    db.PrintingEstimationLaborCosts.Add(entity);
                    db.SaveChanges();
                }
            }
        }

        public static void TextPrintCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            if (estimation.CoverMachineTypeId == printingProcess.PrintingMachineTypeId)
            {
                using (var db = new ExceedDbContext())
                {
                    var machineType = db.PrintingMachineTypes
                        .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId); // text machine

                    if (machineType != null)
                    {
                        var machineDetail = db.PrintingMachineDetails
                            .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                            x.PaperSizeId == estimation.TextPaperSizeId); // text

                        if (machineDetail != null)
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
                            entity.Quantity = (estimation.ProductQuantity * estimation.TextNoOfColors) /
                                (machineDetail.SectionUp * machineType.NumberOfColors);
                            entity.TotalCost = entity.Quantity * entity.LaborRate;
                            db.PrintingEstimationLaborCosts.Add(entity);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        #endregion

        #region PostPress
        public static void CuttingTextCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
            {
                var materialCost = db.PrintingEstimationMaterialCosts
                   .FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId &&
                   x.EstimationMaterialType == EstimationMaterialType.TextPaper); // text paper

                if (materialCost != null)
                {
                    var materialItem = db.PrintingMaterialCategoryItems
                        .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == materialCost.PrintingMaterialCategoryItemId);

                    if (materialItem != null && materialItem.PaperType == PaperType.SheetFed)
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
                            Quantity = materialCost.Quantity,
                        };
                        entity.TotalCost = entity.Quantity * entity.LaborRate;
                        db.PrintingEstimationLaborCosts.Add(entity);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void TrimmingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }

        public static void CheckingPackingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
                    Quantity = estimation.ProductQuantity / estimation.Pack
                };
                entity.TotalCost = entity.Quantity * entity.LaborRate;
                db.PrintingEstimationLaborCosts.Add(entity);
                db.SaveChanges();
            }
        }

        public static void LoadingUnloadingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }

        public static void DispatchingTransportingCost(PrintingCostEstimation estimation, PrintingProcess printingProcess)
        {
            using (var db = new ExceedDbContext())
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
        }
        #endregion
    }
}