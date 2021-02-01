namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.CleanedNaturalGumSellingPriceItem",
                c => new
                    {
                        CleanedNaturalGumSellingPriceItemId = c.Int(nullable: false, identity: true),
                        GumProcessingOrderItemId = c.Int(nullable: false),
                        CleanedNaturalGumSellingPriceId = c.Int(nullable: false),
                        RawQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CleanedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProcessingAndPackingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Recovery = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalNetCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedNaturalGumSellingPriceItemId)
                .ForeignKey("Production.CleanedNaturalGumSellingPrice", t => t.CleanedNaturalGumSellingPriceId, cascadeDelete: true)
                .ForeignKey("Production.GumProcessingOrderItem", t => t.GumProcessingOrderItemId, cascadeDelete: true)
                .Index(t => t.GumProcessingOrderItemId)
                .Index(t => t.CleanedNaturalGumSellingPriceId);
            
            CreateTable(
                "Production.CleanedNaturalGumSellingPrice",
                c => new
                    {
                        CleanedNaturalGumSellingPriceId = c.Int(nullable: false, identity: true),
                        ProductionYearId = c.Int(nullable: false),
                        NaturalGumId = c.Int(nullable: false),
                        SellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedNaturalGumSellingPriceId)
                .ForeignKey("Production.NaturalGum", t => t.NaturalGumId, cascadeDelete: true)
                .ForeignKey("Production.ProductionYear", t => t.ProductionYearId, cascadeDelete: true)
                .Index(t => t.ProductionYearId)
                .Index(t => t.NaturalGumId);
            
            CreateTable(
                "Production.CleanedSeedSellingPriceItem",
                c => new
                    {
                        CleanedSeedSellingPriceItemId = c.Int(nullable: false, identity: true),
                        SeedProcessingOrderItemId = c.Int(nullable: false),
                        CleanedSeedSellingPriceId = c.Int(nullable: false),
                        RawQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CleanedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProcessingAndPackingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Recovery = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalNetCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedSeedSellingPriceItemId)
                .ForeignKey("Production.CleanedSeedSellingPrice", t => t.CleanedSeedSellingPriceId, cascadeDelete: true)
                .ForeignKey("Production.SeedProcessingOrderItem", t => t.SeedProcessingOrderItemId, cascadeDelete: true)
                .Index(t => t.SeedProcessingOrderItemId)
                .Index(t => t.CleanedSeedSellingPriceId);
            
            CreateTable(
                "Production.CleanedSeedSellingPrice",
                c => new
                    {
                        CleanedSeedSellingPriceId = c.Int(nullable: false, identity: true),
                        ProductionYearId = c.Int(nullable: false),
                        CropId = c.Int(nullable: false),
                        SellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedSeedSellingPriceId)
                .ForeignKey("Production.Crop", t => t.CropId, cascadeDelete: true)
                .ForeignKey("Production.ProductionYear", t => t.ProductionYearId, cascadeDelete: true)
                .Index(t => t.ProductionYearId)
                .Index(t => t.CropId);
            
            DropColumn("APTS.DrugStores", "NameWithCode");
        }
        
        public override void Down()
        {
            AddColumn("APTS.DrugStores", "NameWithCode", c => c.String());
            DropForeignKey("Production.CleanedSeedSellingPriceItem", "SeedProcessingOrderItemId", "Production.SeedProcessingOrderItem");
            DropForeignKey("Production.CleanedSeedSellingPriceItem", "CleanedSeedSellingPriceId", "Production.CleanedSeedSellingPrice");
            DropForeignKey("Production.CleanedSeedSellingPrice", "ProductionYearId", "Production.ProductionYear");
            DropForeignKey("Production.CleanedSeedSellingPrice", "CropId", "Production.Crop");
            DropForeignKey("Production.CleanedNaturalGumSellingPriceItem", "GumProcessingOrderItemId", "Production.GumProcessingOrderItem");
            DropForeignKey("Production.CleanedNaturalGumSellingPriceItem", "CleanedNaturalGumSellingPriceId", "Production.CleanedNaturalGumSellingPrice");
            DropForeignKey("Production.CleanedNaturalGumSellingPrice", "ProductionYearId", "Production.ProductionYear");
            DropForeignKey("Production.CleanedNaturalGumSellingPrice", "NaturalGumId", "Production.NaturalGum");
            DropIndex("Production.CleanedSeedSellingPrice", new[] { "CropId" });
            DropIndex("Production.CleanedSeedSellingPrice", new[] { "ProductionYearId" });
            DropIndex("Production.CleanedSeedSellingPriceItem", new[] { "CleanedSeedSellingPriceId" });
            DropIndex("Production.CleanedSeedSellingPriceItem", new[] { "SeedProcessingOrderItemId" });
            DropIndex("Production.CleanedNaturalGumSellingPrice", new[] { "NaturalGumId" });
            DropIndex("Production.CleanedNaturalGumSellingPrice", new[] { "ProductionYearId" });
            DropIndex("Production.CleanedNaturalGumSellingPriceItem", new[] { "CleanedNaturalGumSellingPriceId" });
            DropIndex("Production.CleanedNaturalGumSellingPriceItem", new[] { "GumProcessingOrderItemId" });
            DropTable("Production.CleanedSeedSellingPrice");
            DropTable("Production.CleanedSeedSellingPriceItem");
            DropTable("Production.CleanedNaturalGumSellingPrice");
            DropTable("Production.CleanedNaturalGumSellingPriceItem");
        }
    }
}
