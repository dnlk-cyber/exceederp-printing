namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.CostTrackingPeriod",
                c => new
                    {
                        CostTrackingPeriodId = c.Int(nullable: false, identity: true),
                        ProductionPeriodId = c.Int(nullable: false),
                        ProductionProjectId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CostTrackingPeriodId)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .ForeignKey("Production.ProductionProject", t => t.ProductionProjectId, cascadeDelete: true)
                .Index(t => t.ProductionPeriodId)
                .Index(t => t.ProductionProjectId);
            
            CreateTable(
                "Production.ProductionPeriod",
                c => new
                    {
                        ProductionPeriodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateFromAmharic = c.String(),
                        DateTo = c.DateTime(nullable: false),
                        DateToAmharic = c.String(),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.ProductionProject",
                c => new
                    {
                        ProductionProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionProjectId);
            
            CreateTable(
                "Production.Crop",
                c => new
                    {
                        CropId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CropId);
            
            CreateTable(
                "Production.Plot",
                c => new
                    {
                        PlotId = c.Int(nullable: false, identity: true),
                        PlotCode = c.String(),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Area = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        ProductionSettingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlotId);
            
            CreateTable(
                "Production.ProductionActivity",
                c => new
                    {
                        ProductionActivityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProductionSettingId = c.Int(nullable: false),
                        Equipmentcost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionActivityId)
                .ForeignKey("Production.ProductionSetting", t => t.ProductionSettingId, cascadeDelete: true)
                .Index(t => t.ProductionSettingId);
            
            CreateTable(
                "Production.ProductionSetting",
                c => new
                    {
                        ProductionSettingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionSettingId);
            
            CreateTable(
                "Production.ProductionOrder",
                c => new
                    {
                        ProductionOrderId = c.Int(nullable: false, identity: true),
                        ProductionType = c.Int(nullable: false),
                        PlotId = c.Int(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        ProductionCode = c.String(),
                        Description = c.String(),
                        CropId = c.Int(nullable: false),
                        SeedType = c.Int(nullable: false),
                        Generation = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionOrderId);
            
            CreateTable(
                "Production.ProjectCostByMonth",
                c => new
                    {
                        ProjectCostByMonthId = c.Int(nullable: false, identity: true),
                        CostTrackingPeriodId = c.Int(nullable: false),
                        CropId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Generation = c.Int(nullable: false),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCostByMonthId)
                .ForeignKey("Production.Crop", t => t.CropId, cascadeDelete: true)
                .ForeignKey("Production.CostTrackingPeriod", t => t.CostTrackingPeriodId, cascadeDelete: true)
                .Index(t => t.CostTrackingPeriodId)
                .Index(t => t.CropId);
            
            CreateTable(
                "Production.ProjectCostByMonthDetail",
                c => new
                    {
                        ProjectCostByMonthDetailId = c.Int(nullable: false, identity: true),
                        ProjectCostByMonthId = c.Int(nullable: false),
                        ActivityName = c.String(),
                        OperationType = c.Int(nullable: false),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCostByMonthDetailId)
                .ForeignKey("Production.ProjectCostByMonth", t => t.ProjectCostByMonthId, cascadeDelete: true)
                .Index(t => t.ProjectCostByMonthId);
            
            CreateTable(
                "Production.ProjectCostByMonthMaterial",
                c => new
                    {
                        ProjectCostByMonthMaterialId = c.Int(nullable: false, identity: true),
                        ProjectCostByMonthId = c.Int(nullable: false),
                        MaterialName = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCostByMonthMaterialId)
                .ForeignKey("Production.ProjectCostByMonth", t => t.ProjectCostByMonthId, cascadeDelete: true)
                .Index(t => t.ProjectCostByMonthId);
            
            CreateTable(
                "EXCEED.SeedProductionPlan",
                c => new
                    {
                        SeedProductionPlanId = c.Int(nullable: false, identity: true),
                        SeedProductionPlanYearId = c.Int(nullable: false),
                        CropId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Generation = c.Int(nullable: false),
                        Area = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YieldQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeedProductionPlanId)
                .ForeignKey("Production.Crop", t => t.CropId, cascadeDelete: true)
                .ForeignKey("Production.SeedProductionPlanYear", t => t.SeedProductionPlanYearId, cascadeDelete: true)
                .Index(t => t.SeedProductionPlanYearId)
                .Index(t => t.CropId);
            
            CreateTable(
                "Production.SeedProductionPlanYear",
                c => new
                    {
                        SeedProductionPlanYearId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        ProductionProjectId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeedProductionPlanYearId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("Production.ProductionProject", t => t.ProductionProjectId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.ProductionProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.SeedProductionPlan", "SeedProductionPlanYearId", "Production.SeedProductionPlanYear");
            DropForeignKey("Production.SeedProductionPlanYear", "ProductionProjectId", "Production.ProductionProject");
            DropForeignKey("Production.SeedProductionPlanYear", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.SeedProductionPlan", "CropId", "Production.Crop");
            DropForeignKey("Production.ProjectCostByMonthMaterial", "ProjectCostByMonthId", "Production.ProjectCostByMonth");
            DropForeignKey("Production.ProjectCostByMonthDetail", "ProjectCostByMonthId", "Production.ProjectCostByMonth");
            DropForeignKey("Production.ProjectCostByMonth", "CostTrackingPeriodId", "Production.CostTrackingPeriod");
            DropForeignKey("Production.ProjectCostByMonth", "CropId", "Production.Crop");
            DropForeignKey("Production.ProductionActivity", "ProductionSettingId", "Production.ProductionSetting");
            DropForeignKey("Production.CostTrackingPeriod", "ProductionProjectId", "Production.ProductionProject");
            DropForeignKey("Production.CostTrackingPeriod", "ProductionPeriodId", "Production.ProductionPeriod");
            DropIndex("Production.SeedProductionPlanYear", new[] { "ProductionProjectId" });
            DropIndex("Production.SeedProductionPlanYear", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.SeedProductionPlan", new[] { "CropId" });
            DropIndex("EXCEED.SeedProductionPlan", new[] { "SeedProductionPlanYearId" });
            DropIndex("Production.ProjectCostByMonthMaterial", new[] { "ProjectCostByMonthId" });
            DropIndex("Production.ProjectCostByMonthDetail", new[] { "ProjectCostByMonthId" });
            DropIndex("Production.ProjectCostByMonth", new[] { "CropId" });
            DropIndex("Production.ProjectCostByMonth", new[] { "CostTrackingPeriodId" });
            DropIndex("Production.ProductionActivity", new[] { "ProductionSettingId" });
            DropIndex("Production.CostTrackingPeriod", new[] { "ProductionProjectId" });
            DropIndex("Production.CostTrackingPeriod", new[] { "ProductionPeriodId" });
            DropTable("Production.SeedProductionPlanYear");
            DropTable("EXCEED.SeedProductionPlan");
            DropTable("Production.ProjectCostByMonthMaterial");
            DropTable("Production.ProjectCostByMonthDetail");
            DropTable("Production.ProjectCostByMonth");
            DropTable("Production.ProductionOrder");
            DropTable("Production.ProductionSetting");
            DropTable("Production.ProductionActivity");
            DropTable("Production.Plot");
            DropTable("Production.Crop");
            DropTable("Production.ProductionProject");
            DropTable("Production.ProductionPeriod");
            DropTable("Production.CostTrackingPeriod");
        }
    }
}
