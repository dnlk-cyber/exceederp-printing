namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Production.ProductionOrder", "BranchId", "HRM.Branch");
            DropIndex("Production.ProductionOrder", new[] { "BranchId" });
            CreateTable(
                "Production.ProductionAndProcessingCostCategory",
                c => new
                    {
                        ProductionAndProcessingCostCategoryId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        HasSubCategory = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        IsDeductive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionAndProcessingCostCategoryId);
            
            CreateTable(
                "Production.ProductionSite",
                c => new
                    {
                        ProductionSiteId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BranchId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionSiteId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            AddColumn("Production.GumProcessingOrderItem", "RawQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.GumProcessingOrderItem", "RecoveryRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.ProductionOrder", "ProductionSiteId", c => c.Int(nullable: false));
            AddColumn("Production.Plot", "ProductionSiteId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "RawQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.SeedProcessingOrderItem", "RecoveryRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Production.GumProcessingOrderItem", "DateFrom");
            DropColumn("Production.GumProcessingOrderItem", "DateTo");
            DropColumn("Production.SeedProcessingOrderItem", "DateFrom");
            DropColumn("Production.SeedProcessingOrderItem", "DateTo");
            DropColumn("Production.SeedProductionOrderItem", "SeedType");
            DropColumn("Production.SeedProductionOrderItem", "Generation");
        }
        
        public override void Down()
        {
            AddColumn("Production.SeedProductionOrderItem", "Generation", c => c.Int(nullable: false));
            AddColumn("Production.SeedProductionOrderItem", "SeedType", c => c.Int(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "DateTo", c => c.DateTime(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "DateFrom", c => c.DateTime(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "DateTo", c => c.DateTime(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "DateFrom", c => c.DateTime(nullable: false));
            DropForeignKey("Production.ProductionSite", "BranchId", "HRM.Branch");
            DropIndex("Production.ProductionSite", new[] { "BranchId" });
            DropColumn("Production.SeedProcessingOrderItem", "RecoveryRate");
            DropColumn("Production.SeedProcessingOrderItem", "RawQuantity");
            DropColumn("Production.Plot", "ProductionSiteId");
            DropColumn("Production.ProductionOrder", "ProductionSiteId");
            DropColumn("Production.GumProcessingOrderItem", "RecoveryRate");
            DropColumn("Production.GumProcessingOrderItem", "RawQuantity");
            DropTable("Production.ProductionSite");
            DropTable("Production.ProductionAndProcessingCostCategory");
            CreateIndex("Production.ProductionOrder", "BranchId");
            AddForeignKey("Production.ProductionOrder", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
    }
}
