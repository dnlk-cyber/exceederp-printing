namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class restructuringprinting : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "JobCosting.DailyLaborRates", newSchema: "Printing");
            MoveTable(name: "JobCosting.DeliveredJobs", newSchema: "Printing");
            MoveTable(name: "JobCosting.DLRHours", newSchema: "Printing");
            MoveTable(name: "JobCosting.DLRSalaries", newSchema: "Printing");
            MoveTable(name: "PrintingEstimation.EstimationForms", newSchema: "Printing");
            MoveTable(name: "PrintingEstimation.LaborCosts", newSchema: "Printing");
            MoveTable(name: "PrintingEstimation.PrintingSteps", newSchema: "Printing");
            MoveTable(name: "PrintingEstimation.MaterialCosts", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.FinishedGoodItems", newSchema: "Printing");
            MoveTable(name: "JobCosting.FinishedJobs", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.HalfYearlyProductionPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.QuarterlyProductionPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.MonthlyProductionPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.WeeklyProductionPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.YearlyProductionPlans", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeCategories", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeNonProductives", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimes", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeNonProductiveNonControllables", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeOTs", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeProductives", newSchema: "Printing");
            MoveTable(name: "JobCosting.JcPensions", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.JobCategories", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobChangeOfOrders", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobLaborCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobMaterialCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobMaterialReturns", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.JobOrderMaterial", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderSummaryCostCenterDetail", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderSummary", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderTimeRegistration", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderTimeSheet", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.Job", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.JobStatuses", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.JobStatusLineItems", newSchema: "Printing");
            MoveTable(name: "JobCosting.Machines", newSchema: "Printing");
            MoveTable(name: "PrintingEstimation.OverallCosts", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.PFProformaItems", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.FinishedGoodsStoreReceiveDistribution", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.FinishedGoodsStoreReceive", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.FinishedGoodsStoreReceiveItem", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.FinishedGoodsStoreReceiveValidation", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductDelivery", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductDeliveryItems", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductionCustomerTypes", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductionJobTypePlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductionMonthlyJobTypePlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductionMonthlyPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.ProductionYearlyPlans", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.Proforma", newSchema: "Printing");
            MoveTable(name: "JobCosting.YearlyOverHeadRates", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.Graphic", newSchema: "Printing");
            MoveTable(name: "ProductFollowUp.Settings", newSchema: "Printing");
            DropForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCategoryId", "PrintingEstimation.FurnitureCategories");
            DropForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropForeignKey("PrintingEstimation.FurnitureMaterialCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropForeignKey("PrintingEstimation.FurnitureOverallCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropIndex("PrintingEstimation.DirectLaborCosts", new[] { "FurnitureCategoryId" });
            DropIndex("PrintingEstimation.DirectLaborCosts", new[] { "FurnitureCostEstimationId" });
            DropIndex("PrintingEstimation.FurnitureMaterialCosts", new[] { "FurnitureCostEstimationId" });
            DropIndex("PrintingEstimation.FurnitureOverallCosts", new[] { "FurnitureCostEstimationId" });
            AddColumn("Printing.JobOrderTimeSheet", "FurnitureJobOrderTimeRegistrationId", c => c.Int(nullable: false));
            DropTable("PrintingEstimation.DirectLaborCosts");
            DropTable("PrintingEstimation.FurnitureCategories");
            DropTable("PrintingEstimation.FurnitureCostEstimations");
            DropTable("PrintingEstimation.FurnitureMaterialCosts");
            DropTable("PrintingEstimation.FurnitureOverallCosts");
        }
        
        public override void Down()
        {
            CreateTable(
                "PrintingEstimation.FurnitureOverallCosts",
                c => new
                    {
                        FurnitureOverallCostId = c.Int(nullable: false, identity: true),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MarginalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NBT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TInpercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Graphic = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedNetPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureOverallCostId);
            
            CreateTable(
                "PrintingEstimation.FurnitureMaterialCosts",
                c => new
                    {
                        FurnitureMaterialCostId = c.Int(nullable: false, identity: true),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                        InventoryCategoryId = c.String(),
                        InventoryId = c.String(),
                        JobId = c.Int(),
                        StoreCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureMaterialCostId);
            
            CreateTable(
                "PrintingEstimation.FurnitureCostEstimations",
                c => new
                    {
                        FurnitureCostEstimationId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JT = c.String(),
                        JobNumber = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Others = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        CheckedBy = c.String(),
                        ApprovedBy = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureCostEstimationId);
            
            CreateTable(
                "PrintingEstimation.FurnitureCategories",
                c => new
                    {
                        FurnitureCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureCategoryId);
            
            CreateTable(
                "PrintingEstimation.DirectLaborCosts",
                c => new
                    {
                        DirectLaborCostId = c.Int(nullable: false, identity: true),
                        FurnitureCategoryId = c.Int(nullable: false),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DirectLaborCostId);
            
            DropColumn("Printing.JobOrderTimeSheet", "FurnitureJobOrderTimeRegistrationId");
            CreateIndex("PrintingEstimation.FurnitureOverallCosts", "FurnitureCostEstimationId");
            CreateIndex("PrintingEstimation.FurnitureMaterialCosts", "FurnitureCostEstimationId");
            CreateIndex("PrintingEstimation.DirectLaborCosts", "FurnitureCostEstimationId");
            CreateIndex("PrintingEstimation.DirectLaborCosts", "FurnitureCategoryId");
            AddForeignKey("PrintingEstimation.FurnitureOverallCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations", "FurnitureCostEstimationId", cascadeDelete: true);
            AddForeignKey("PrintingEstimation.FurnitureMaterialCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations", "FurnitureCostEstimationId", cascadeDelete: true);
            AddForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations", "FurnitureCostEstimationId", cascadeDelete: true);
            AddForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCategoryId", "PrintingEstimation.FurnitureCategories", "FurnitureCategoryId", cascadeDelete: true);
            MoveTable(name: "Printing.Settings", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.Graphic", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.YearlyOverHeadRates", newSchema: "JobCosting");
            MoveTable(name: "Printing.Proforma", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductionYearlyPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductionMonthlyPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductionMonthlyJobTypePlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductionJobTypePlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductionCustomerTypes", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductDeliveryItems", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.ProductDelivery", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.FinishedGoodsStoreReceiveValidation", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.FinishedGoodsStoreReceiveItem", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.FinishedGoodsStoreReceive", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.FinishedGoodsStoreReceiveDistribution", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.PFProformaItems", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.OverallCosts", newSchema: "PrintingEstimation");
            MoveTable(name: "Printing.Machines", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobStatusLineItems", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.JobStatuses", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.Job", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.JobOrderTimeSheet", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderTimeRegistration", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderSummary", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderSummaryCostCenterDetail", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderMaterial", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.JobMaterialReturns", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobMaterialCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobLaborCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobChangeOfOrders", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobCategories", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.JcPensions", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeProductives", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeOTs", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeNonProductiveNonControllables", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimes", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeNonProductives", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeCategories", newSchema: "JobCosting");
            MoveTable(name: "Printing.YearlyProductionPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.WeeklyProductionPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.MonthlyProductionPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.QuarterlyProductionPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.HalfYearlyProductionPlans", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.FinishedJobs", newSchema: "JobCosting");
            MoveTable(name: "Printing.FinishedGoodItems", newSchema: "ProductFollowUp");
            MoveTable(name: "Printing.MaterialCosts", newSchema: "PrintingEstimation");
            MoveTable(name: "Printing.PrintingSteps", newSchema: "PrintingEstimation");
            MoveTable(name: "Printing.LaborCosts", newSchema: "PrintingEstimation");
            MoveTable(name: "Printing.EstimationForms", newSchema: "PrintingEstimation");
            MoveTable(name: "Printing.DLRSalaries", newSchema: "JobCosting");
            MoveTable(name: "Printing.DLRHours", newSchema: "JobCosting");
            MoveTable(name: "Printing.DeliveredJobs", newSchema: "JobCosting");
            MoveTable(name: "Printing.DailyLaborRates", newSchema: "JobCosting");
        }
    }
}
