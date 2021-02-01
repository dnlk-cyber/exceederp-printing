namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.NaturalGum",
                c => new
                    {
                        NaturalGumId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NaturalGumId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.ProductionStoreReceiveItem",
                c => new
                    {
                        ProductionStoreReceiveItemId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        ProductionStoreReceiveId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UOM = c.String(),
                        AccountCode = c.String(),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionStoreReceiveItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .ForeignKey("Production.ProductionStoreReceive", t => t.ProductionStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.ProductionStoreReceiveId);
            
            CreateTable(
                "Production.ProductionStoreReceive",
                c => new
                    {
                        ProductionStoreReceiveId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
                        ProductType = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionStoreReceiveId);
            
            CreateTable(
                "Production.ProductionStoreReceiveValidation",
                c => new
                    {
                        ProductionStoreReceiveValidationId = c.Int(nullable: false, identity: true),
                        ProductionStoreReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionStoreReceiveValidationId)
                .ForeignKey("Production.ProductionStoreReceive", t => t.ProductionStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ProductionStoreReceiveId);
            
            AddColumn("Production.Crop", "ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("Production.Crop", "ItemCode", c => c.String(maxLength: 128));
            AddColumn("Production.Crop", "Description", c => c.String());
            AddColumn("Production.GumProductionOrderItem", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Production.GumProductionOrderItem", "NaturalGumId", c => c.Int(nullable: false));
            AddColumn("Production.GumProductionOrderItem", "PurchaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.SeedProductionOrderItem", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProductionOrderItem", "PurchaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("Production.Crop", "ItemCategoryCode");
            CreateIndex("Production.Crop", "ItemCode");
            AddForeignKey("Production.Crop", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("Production.Crop", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            DropColumn("Production.GumProductionOrderItem", "ProductType");
            DropColumn("Production.SeedProductionOrderItem", "DateFrom");
            DropColumn("Production.SeedProductionOrderItem", "DateTo");
        }
        
        public override void Down()
        {
            AddColumn("Production.SeedProductionOrderItem", "DateTo", c => c.DateTime(nullable: false));
            AddColumn("Production.SeedProductionOrderItem", "DateFrom", c => c.DateTime(nullable: false));
            AddColumn("Production.GumProductionOrderItem", "ProductType", c => c.String());
            DropForeignKey("Production.ProductionStoreReceiveValidation", "ProductionStoreReceiveId", "Production.ProductionStoreReceive");
            DropForeignKey("Production.ProductionStoreReceiveItem", "ProductionStoreReceiveId", "Production.ProductionStoreReceive");
            DropForeignKey("Production.ProductionStoreReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProductionStoreReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.NaturalGum", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.NaturalGum", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.Crop", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.Crop", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Production.ProductionStoreReceiveValidation", new[] { "ProductionStoreReceiveId" });
            DropIndex("Production.ProductionStoreReceiveItem", new[] { "ProductionStoreReceiveId" });
            DropIndex("Production.ProductionStoreReceiveItem", new[] { "ItemCode" });
            DropIndex("Production.ProductionStoreReceiveItem", new[] { "ItemCategoryCode" });
            DropIndex("Production.NaturalGum", new[] { "ItemCode" });
            DropIndex("Production.NaturalGum", new[] { "ItemCategoryCode" });
            DropIndex("Production.Crop", new[] { "ItemCode" });
            DropIndex("Production.Crop", new[] { "ItemCategoryCode" });
            DropColumn("Production.SeedProductionOrderItem", "PurchaseCost");
            DropColumn("Production.SeedProductionOrderItem", "GlFiscalYearId");
            DropColumn("Production.GumProductionOrderItem", "PurchaseCost");
            DropColumn("Production.GumProductionOrderItem", "NaturalGumId");
            DropColumn("Production.GumProductionOrderItem", "GlFiscalYearId");
            DropColumn("Production.Crop", "Description");
            DropColumn("Production.Crop", "ItemCode");
            DropColumn("Production.Crop", "ItemCategoryCode");
            DropTable("Production.ProductionStoreReceiveValidation");
            DropTable("Production.ProductionStoreReceive");
            DropTable("Production.ProductionStoreReceiveItem");
            DropTable("Production.NaturalGum");
        }
    }
}
