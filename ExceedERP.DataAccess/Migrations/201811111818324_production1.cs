namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.GumProcessingOrderItem",
                c => new
                    {
                        GumProcessingOrderItemId = c.Int(nullable: false, identity: true),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        ProcessingPlantId = c.Int(nullable: false),
                        GumName = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        ProcessingCode = c.String(),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityProduced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfOutPutProduced = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GumProcessingOrderItemId)
                .ForeignKey("Production.GumProductionOrderItem", t => t.GumProductionOrderItemId, cascadeDelete: true)
                .ForeignKey("Production.ProcessingPlant", t => t.ProcessingPlantId, cascadeDelete: true)
                .Index(t => t.GumProductionOrderItemId)
                .Index(t => t.ProcessingPlantId);
            
            CreateTable(
                "Production.GumProductionOrderItem",
                c => new
                    {
                        GumProductionOrderItemId = c.Int(nullable: false, identity: true),
                        ProductionOrderId = c.Int(nullable: false),
                        ProductType = c.String(),
                        ProductionType = c.Int(nullable: false),
                        ProductionCode = c.String(),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityProduced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfOutPutProduced = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GumProductionOrderItemId)
                .ForeignKey("Production.ProductionOrder", t => t.ProductionOrderId, cascadeDelete: true)
                .Index(t => t.ProductionOrderId);
            
            CreateTable(
                "Production.ProcessingPlant",
                c => new
                    {
                        ProcessingPlantId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        PlantType = c.Int(nullable: false),
                        Name = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcessingPlantId);
            
            CreateTable(
                "Production.ProcessingInputMaterial",
                c => new
                    {
                        ProcessingInputMaterialId = c.Int(nullable: false, identity: true),
                        GumProcessingOrderItemId = c.Int(nullable: false),
                        SeedProcessingOrderItemId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPeriodId = c.Int(nullable: false),
                        DateOfInformation = c.DateTime(nullable: false),
                        BugdetYear = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcessingInputMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.ProcessingOrderActivity",
                c => new
                    {
                        ProcessingOrderActivityId = c.Int(nullable: false, identity: true),
                        GumProcessingOrderItemId = c.Int(nullable: false),
                        SeedProcessingOrderItemId = c.Int(nullable: false),
                        ProductionActivityId = c.Int(nullable: false),
                        OperationType = c.Int(nullable: false),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPeriodId = c.Int(nullable: false),
                        DateOfInformation = c.DateTime(nullable: false),
                        BugdetYear = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcessingOrderActivityId)
                .ForeignKey("Production.ProductionActivity", t => t.ProductionActivityId, cascadeDelete: true)
                .Index(t => t.ProductionActivityId);
            
            CreateTable(
                "Production.ProductionItemCategory",
                c => new
                    {
                        ProductionItemCategoryId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        Name = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionItemCategoryId)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode);
            
            CreateTable(
                "Production.ProductionOrderActivitiy",
                c => new
                    {
                        ProductionOrderActivitiyId = c.Int(nullable: false, identity: true),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        ProductionActivityId = c.Int(nullable: false),
                        OperationType = c.Int(nullable: false),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPeriodId = c.Int(nullable: false),
                        DateOfInformation = c.DateTime(nullable: false),
                        BugdetYear = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionOrderActivitiyId)
                .ForeignKey("Production.ProductionActivity", t => t.ProductionActivityId, cascadeDelete: true)
                .Index(t => t.ProductionActivityId);
            
            CreateTable(
                "Production.ProductionOrderInputMaterial",
                c => new
                    {
                        ProductionOrderInputMaterialId = c.Int(nullable: false, identity: true),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPeriodId = c.Int(nullable: false),
                        DateOfInformation = c.DateTime(nullable: false),
                        BugdetYear = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionOrderInputMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.SeedProcessingOrderItem",
                c => new
                    {
                        SeedProcessingOrderItemId = c.Int(nullable: false, identity: true),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        ProcessingPlantId = c.Int(nullable: false),
                        SeedName = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        ProcessingCode = c.String(),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityProduced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfOutPutProduced = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeedProcessingOrderItemId)
                .ForeignKey("Production.ProcessingPlant", t => t.ProcessingPlantId, cascadeDelete: true)
                .ForeignKey("Production.SeedProductionOrderItem", t => t.SeedProductionOrderItemId, cascadeDelete: true)
                .Index(t => t.SeedProductionOrderItemId)
                .Index(t => t.ProcessingPlantId);
            
            CreateTable(
                "Production.SeedProductionOrderItem",
                c => new
                    {
                        SeedProductionOrderItemId = c.Int(nullable: false, identity: true),
                        ProductionOrderId = c.Int(nullable: false),
                        PlotId = c.Int(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        ProductionCode = c.String(),
                        Description = c.String(),
                        ProductionType = c.Int(nullable: false),
                        CropId = c.Int(nullable: false),
                        SeedType = c.Int(nullable: false),
                        Generation = c.Int(nullable: false),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityProduced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfOutPutProduced = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeedProductionOrderItemId)
                .ForeignKey("Production.ProductionOrder", t => t.ProductionOrderId, cascadeDelete: true)
                .Index(t => t.ProductionOrderId);
            
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "HourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "UnitOfMeasurementId", c => c.Int());
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "OverheadRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "ProfitRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "HourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "UnitOfMeasurementId", c => c.Int());
            AddColumn("Construction.GenericProjectCustomWorkItem", "OverheadRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "ProfitRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.Plot", "IsolationDistance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionOrder", "BranchId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectCustomWorkItem", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "HourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "UnitOfMeasurementId", c => c.Int());
            AddColumn("Construction.ProjectCustomWorkItem", "OverheadRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "ProfitRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("Production.ProductionOrder", "BranchId");
            AddForeignKey("Production.ProductionOrder", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            DropColumn("Construction.EstimationCustomWorkItemEquipment", "EquipmentUnitCost");
            DropColumn("Construction.EstimationCustomWorkItemEquipment", "EquipmentType");
            DropColumn("Construction.GenericProjectCustomWorkItem", "Quantity");
            DropColumn("Production.Plot", "Width");
            DropColumn("Production.Plot", "Length");
            DropColumn("Production.ProductionOrder", "PlotId");
            DropColumn("Production.ProductionOrder", "DateFrom");
            DropColumn("Production.ProductionOrder", "DateTo");
            DropColumn("Production.ProductionOrder", "ProductionCode");
            DropColumn("Production.ProductionOrder", "CropId");
            DropColumn("Production.ProductionOrder", "SeedType");
            DropColumn("Production.ProductionOrder", "Generation");
        }
        
        public override void Down()
        {
            AddColumn("Production.ProductionOrder", "Generation", c => c.Int(nullable: false));
            AddColumn("Production.ProductionOrder", "SeedType", c => c.Int(nullable: false));
            AddColumn("Production.ProductionOrder", "CropId", c => c.Int(nullable: false));
            AddColumn("Production.ProductionOrder", "ProductionCode", c => c.String());
            AddColumn("Production.ProductionOrder", "DateTo", c => c.DateTime(nullable: false));
            AddColumn("Production.ProductionOrder", "DateFrom", c => c.DateTime(nullable: false));
            AddColumn("Production.ProductionOrder", "PlotId", c => c.Int(nullable: false));
            AddColumn("Production.Plot", "Length", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.Plot", "Width", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericProjectCustomWorkItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.EstimationCustomWorkItemEquipment", "EquipmentType", c => c.String());
            AddColumn("Construction.EstimationCustomWorkItemEquipment", "EquipmentUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId", "Production.SeedProductionOrderItem");
            DropForeignKey("Production.SeedProductionOrderItem", "ProductionOrderId", "Production.ProductionOrder");
            DropForeignKey("Production.SeedProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant");
            DropForeignKey("Production.ProductionOrderInputMaterial", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProductionOrderInputMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.ProductionOrderActivitiy", "ProductionActivityId", "Production.ProductionActivity");
            DropForeignKey("Production.ProductionItemCategory", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProcessingOrderActivity", "ProductionActivityId", "Production.ProductionActivity");
            DropForeignKey("Production.ProcessingInputMaterial", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProcessingInputMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.GumProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant");
            DropForeignKey("Production.GumProcessingOrderItem", "GumProductionOrderItemId", "Production.GumProductionOrderItem");
            DropForeignKey("Production.GumProductionOrderItem", "ProductionOrderId", "Production.ProductionOrder");
            DropForeignKey("Production.ProductionOrder", "BranchId", "HRM.Branch");
            DropIndex("Production.SeedProductionOrderItem", new[] { "ProductionOrderId" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "ProcessingPlantId" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "SeedProductionOrderItemId" });
            DropIndex("Production.ProductionOrderInputMaterial", new[] { "ItemCode" });
            DropIndex("Production.ProductionOrderInputMaterial", new[] { "ItemCategoryCode" });
            DropIndex("Production.ProductionOrderActivitiy", new[] { "ProductionActivityId" });
            DropIndex("Production.ProductionItemCategory", new[] { "ItemCategoryCode" });
            DropIndex("Production.ProcessingOrderActivity", new[] { "ProductionActivityId" });
            DropIndex("Production.ProcessingInputMaterial", new[] { "ItemCode" });
            DropIndex("Production.ProcessingInputMaterial", new[] { "ItemCategoryCode" });
            DropIndex("Production.ProductionOrder", new[] { "BranchId" });
            DropIndex("Production.GumProductionOrderItem", new[] { "ProductionOrderId" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "ProcessingPlantId" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "GumProductionOrderItemId" });
            DropColumn("Construction.ProjectCustomWorkItem", "ProfitCost");
            DropColumn("Construction.ProjectCustomWorkItem", "OverHeadCost");
            DropColumn("Construction.ProjectCustomWorkItem", "DirectCost");
            DropColumn("Construction.ProjectCustomWorkItem", "LaborCostPerUnit");
            DropColumn("Construction.ProjectCustomWorkItem", "EquipmentCostPerUnit");
            DropColumn("Construction.ProjectCustomWorkItem", "MaterialCostPerUnit");
            DropColumn("Construction.ProjectCustomWorkItem", "ProfitRate");
            DropColumn("Construction.ProjectCustomWorkItem", "OverheadRate");
            DropColumn("Construction.ProjectCustomWorkItem", "UnitOfMeasurementId");
            DropColumn("Construction.ProjectCustomWorkItem", "HourlyOutPut");
            DropColumn("Construction.ProjectCustomWorkItem", "TotalQuantityOfWork");
            DropColumn("Production.ProductionOrder", "BranchId");
            DropColumn("Production.Plot", "IsolationDistance");
            DropColumn("Construction.GenericProjectCustomWorkItem", "ProfitCost");
            DropColumn("Construction.GenericProjectCustomWorkItem", "OverHeadCost");
            DropColumn("Construction.GenericProjectCustomWorkItem", "DirectCost");
            DropColumn("Construction.GenericProjectCustomWorkItem", "LaborCostPerUnit");
            DropColumn("Construction.GenericProjectCustomWorkItem", "EquipmentCostPerUnit");
            DropColumn("Construction.GenericProjectCustomWorkItem", "MaterialCostPerUnit");
            DropColumn("Construction.GenericProjectCustomWorkItem", "ProfitRate");
            DropColumn("Construction.GenericProjectCustomWorkItem", "OverheadRate");
            DropColumn("Construction.GenericProjectCustomWorkItem", "UnitOfMeasurementId");
            DropColumn("Construction.GenericProjectCustomWorkItem", "HourlyOutPut");
            DropColumn("Construction.GenericProjectCustomWorkItem", "TotalQuantityOfWork");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "ProfitCost");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "OverHeadCost");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "DirectCost");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "LaborCostPerUnit");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "EquipmentCostPerUnit");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "MaterialCostPerUnit");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "ProfitRate");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "OverheadRate");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "UnitOfMeasurementId");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "HourlyOutPut");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "TotalQuantityOfWork");
            DropTable("Production.SeedProductionOrderItem");
            DropTable("Production.SeedProcessingOrderItem");
            DropTable("Production.ProductionOrderInputMaterial");
            DropTable("Production.ProductionOrderActivitiy");
            DropTable("Production.ProductionItemCategory");
            DropTable("Production.ProcessingOrderActivity");
            DropTable("Production.ProcessingInputMaterial");
            DropTable("Production.ProcessingPlant");
            DropTable("Production.GumProductionOrderItem");
            DropTable("Production.GumProcessingOrderItem");
        }
    }
}
