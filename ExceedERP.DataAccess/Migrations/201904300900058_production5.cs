namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.InputMaterialApplicationRateItem",
                c => new
                    {
                        InputMaterialApplicationRateItemId = c.Int(nullable: false, identity: true),
                        InputMaterialApplicationRateId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        StandardRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InputMaterialApplicationRateItemId)
                .ForeignKey("Production.InputMaterialApplicationRate", t => t.InputMaterialApplicationRateId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.InputMaterialApplicationRateId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.InputMaterialApplicationRate",
                c => new
                    {
                        InputMaterialApplicationRateId = c.Int(nullable: false, identity: true),
                        ApplicationPlace = c.Int(nullable: false),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InputMaterialApplicationRateId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.ProductionRawProductPurchase",
                c => new
                    {
                        ProductionRawProductPurchaseId = c.Int(nullable: false, identity: true),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        SupplierName = c.String(),
                        PurchasedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionRawProductPurchaseId);
            
            CreateTable(
                "Production.ProductionYear",
                c => new
                    {
                        ProductionYearId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionYearId);
            
            AddColumn("Production.GumProductionOrderItem", "ProductionYearId", c => c.Int(nullable: false));
            AddColumn("Production.GumProductionOrderItem", "Status", c => c.Int(nullable: false));
            AddColumn("Production.MachineryMaintenance", "Reference", c => c.String());
            AddColumn("Production.ProcessingInputMaterial", "Reference", c => c.String());
            AddColumn("Production.ProcessingOrderActivity", "Quantity", c => c.Int(nullable: false));
            AddColumn("Production.ProcessingOrderActivity", "Reference", c => c.String());
            AddColumn("Production.ProductionActivity", "OperationType", c => c.Int(nullable: false));
            AddColumn("Production.ProductionOrderActivitiy", "Quantity", c => c.Int(nullable: false));
            AddColumn("Production.ProductionOrderActivitiy", "Reference", c => c.String());
            AddColumn("Production.ProductionOrderInputMaterial", "Reference", c => c.String());
            AddColumn("Production.SeedProductionOrderItem", "ProductionYearId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProductionOrderItem", "Status", c => c.Int(nullable: false));
            DropColumn("Production.GumProcessingOrderItem", "ProfitCost");
            DropColumn("Production.GumProductionOrderItem", "GlFiscalYearId");
            DropColumn("Production.GumProductionOrderItem", "ProfitCost");
            DropColumn("Production.MachineryMaintenance", "BugdetYear");
            DropColumn("Production.ProcessingInputMaterial", "BugdetYear");
            DropColumn("Production.ProcessingOrderActivity", "BugdetYear");
            DropColumn("Production.ProductionActivity", "Equipmentcost");
            DropColumn("Production.ProductionActivity", "MaterialCost");
            DropColumn("Production.ProductionActivity", "LaborCost");
            DropColumn("Production.ProductionActivity", "OverheadCost");
            DropColumn("Production.ProductionActivity", "ProfitCost");
            DropColumn("Production.ProductionActivity", "TotalCost");
            DropColumn("Production.ProductionOrderActivitiy", "BugdetYear");
            DropColumn("Production.ProductionOrderInputMaterial", "BugdetYear");
            DropColumn("Production.SeedProcessingOrderItem", "ProfitCost");
            DropColumn("Production.SeedProductionOrderItem", "GlFiscalYearId");
            DropColumn("Production.SeedProductionOrderItem", "ProfitCost");
        }
        
        public override void Down()
        {
            AddColumn("Production.SeedProductionOrderItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.SeedProductionOrderItem", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionOrderInputMaterial", "BugdetYear", c => c.String());
            AddColumn("Production.ProductionOrderActivitiy", "BugdetYear", c => c.String());
            AddColumn("Production.ProductionActivity", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionActivity", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionActivity", "OverheadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionActivity", "LaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionActivity", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionActivity", "Equipmentcost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProcessingOrderActivity", "BugdetYear", c => c.String());
            AddColumn("Production.ProcessingInputMaterial", "BugdetYear", c => c.String());
            AddColumn("Production.MachineryMaintenance", "BugdetYear", c => c.String());
            AddColumn("Production.GumProductionOrderItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.GumProductionOrderItem", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("Production.InputMaterialApplicationRateItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.InputMaterialApplicationRateItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.InputMaterialApplicationRateItem", "InputMaterialApplicationRateId", "Production.InputMaterialApplicationRate");
            DropForeignKey("Production.InputMaterialApplicationRate", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.InputMaterialApplicationRate", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Production.InputMaterialApplicationRate", new[] { "ItemCode" });
            DropIndex("Production.InputMaterialApplicationRate", new[] { "ItemCategoryCode" });
            DropIndex("Production.InputMaterialApplicationRateItem", new[] { "ItemCode" });
            DropIndex("Production.InputMaterialApplicationRateItem", new[] { "ItemCategoryCode" });
            DropIndex("Production.InputMaterialApplicationRateItem", new[] { "InputMaterialApplicationRateId" });
            DropColumn("Production.SeedProductionOrderItem", "Status");
            DropColumn("Production.SeedProductionOrderItem", "ProductionYearId");
            DropColumn("Production.ProductionOrderInputMaterial", "Reference");
            DropColumn("Production.ProductionOrderActivitiy", "Reference");
            DropColumn("Production.ProductionOrderActivitiy", "Quantity");
            DropColumn("Production.ProductionActivity", "OperationType");
            DropColumn("Production.ProcessingOrderActivity", "Reference");
            DropColumn("Production.ProcessingOrderActivity", "Quantity");
            DropColumn("Production.ProcessingInputMaterial", "Reference");
            DropColumn("Production.MachineryMaintenance", "Reference");
            DropColumn("Production.GumProductionOrderItem", "Status");
            DropColumn("Production.GumProductionOrderItem", "ProductionYearId");
            DropTable("Production.ProductionYear");
            DropTable("Production.ProductionRawProductPurchase");
            DropTable("Production.InputMaterialApplicationRate");
            DropTable("Production.InputMaterialApplicationRateItem");
        }
    }
}
