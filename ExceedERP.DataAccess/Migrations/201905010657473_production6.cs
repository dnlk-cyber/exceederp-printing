namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.ProductionFinishedGoodsStoreReceiveItem",
                c => new
                    {
                        ProductionFinishedGoodsStoreReceiveItemId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        ProductionFinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UOM = c.String(),
                        AccountCode = c.String(),
                        SeedProcessingOrderItemId = c.Int(nullable: false),
                        GumProcessingOrderItemId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionFinishedGoodsStoreReceiveItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .ForeignKey("Production.ProductionFinishedGoodsStoreReceive", t => t.ProductionFinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.ProductionFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "Production.ProductionFinishedGoodsStoreReceive",
                c => new
                    {
                        ProductionFinishedGoodsStoreReceiveId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.ProductionFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "Production.ProductionFinishedGoodsStoreReceiveValidation",
                c => new
                    {
                        ProductionFinishedGoodsStoreReceiveValidationId = c.Int(nullable: false, identity: true),
                        ProductionFinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionFinishedGoodsStoreReceiveValidationId)
                .ForeignKey("Production.ProductionFinishedGoodsStoreReceive", t => t.ProductionFinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ProductionFinishedGoodsStoreReceiveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Production.ProductionFinishedGoodsStoreReceiveValidation", "ProductionFinishedGoodsStoreReceiveId", "Production.ProductionFinishedGoodsStoreReceive");
            DropForeignKey("Production.ProductionFinishedGoodsStoreReceiveItem", "ProductionFinishedGoodsStoreReceiveId", "Production.ProductionFinishedGoodsStoreReceive");
            DropForeignKey("Production.ProductionFinishedGoodsStoreReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProductionFinishedGoodsStoreReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Production.ProductionFinishedGoodsStoreReceiveValidation", new[] { "ProductionFinishedGoodsStoreReceiveId" });
            DropIndex("Production.ProductionFinishedGoodsStoreReceiveItem", new[] { "ProductionFinishedGoodsStoreReceiveId" });
            DropIndex("Production.ProductionFinishedGoodsStoreReceiveItem", new[] { "ItemCode" });
            DropIndex("Production.ProductionFinishedGoodsStoreReceiveItem", new[] { "ItemCategoryCode" });
            DropTable("Production.ProductionFinishedGoodsStoreReceiveValidation");
            DropTable("Production.ProductionFinishedGoodsStoreReceive");
            DropTable("Production.ProductionFinishedGoodsStoreReceiveItem");
        }
    }
}
