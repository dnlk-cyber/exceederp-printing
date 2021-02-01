namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetInspection",
                c => new
                    {
                        FixedAssetInspectionId = c.Int(nullable: false, identity: true),
                        InspectionDate = c.DateTime(nullable: false),
                        StoreCode = c.String(nullable: false),
                        CommitteeMember1 = c.String(),
                        CommitteeMember2 = c.String(),
                        CommitteeMember3 = c.String(),
                        CommitteeDecision = c.String(nullable: false),
                        GoodReceiveId = c.Int(nullable: false),
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
                        FixedAssetReceive_FixedAssetReceiveId = c.Int(),
                    })
                .PrimaryKey(t => t.FixedAssetInspectionId)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetReceive", t => t.FixedAssetReceive_FixedAssetReceiveId)
                .Index(t => t.GoodReceiveId)
                .Index(t => t.FixedAssetReceive_FixedAssetReceiveId);
            
            CreateTable(
                "EXCEED.FixedAssetReceiveDistribution",
                c => new
                    {
                        FixedAssetReceiveDistributionID = c.Int(nullable: false, identity: true),
                        FixedAssetReceiveId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FixedAssetReceiveDistributionID)
                .ForeignKey("EXCEED.FixedAssetReceive", t => t.FixedAssetReceiveId, cascadeDelete: true)
                .Index(t => t.FixedAssetReceiveId);
            
            CreateTable(
                "EXCEED.FixedAssetReceive",
                c => new
                    {
                        FixedAssetReceiveId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        ReceiveTypeId = c.Int(nullable: false),
                        ReceiveRef = c.String(),
                        StoreCode = c.String(nullable: false),
                        SupplierName = c.String(),
                        SupplierInvoiceNo = c.String(),
                        Reference = c.String(),
                        TenderLot = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.FixedAssetReceiveId)
                .ForeignKey("EXCEED.ReceiveType", t => t.ReceiveTypeId, cascadeDelete: true)
                .Index(t => t.ReceiveTypeId);
            
            CreateTable(
                "EXCEED.FixedAssetReceiveItem",
                c => new
                    {
                        FixedAssetReceiveItemID = c.Int(nullable: false, identity: true),
                        FixedAssetReceiveId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsExtra = c.Boolean(nullable: false),
                        ExtraQuantity = c.Decimal(precision: 18, scale: 2),
                        ExtraAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyInformation = c.String(),
                        Specification = c.String(),
                        Expire = c.Boolean(nullable: false),
                        ExpireDate = c.DateTime(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMachinery = c.Boolean(nullable: false),
                        BatchNo = c.String(),
                        Model = c.String(),
                        SerialNo = c.String(),
                        Condition = c.String(),
                        PlateNo = c.String(),
                        ChassisNo = c.String(),
                        EngineNo = c.String(),
                        EngineModelNo = c.String(),
                        EngineElectricalNo = c.String(),
                        EngineElectricalModelNo = c.String(),
                        UOM = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReceiveItemID)
                .ForeignKey("EXCEED.FixedAssetReceive", t => t.FixedAssetReceiveId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.FixedAssetReceiveId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.FixedAssetReceiveValidation",
                c => new
                    {
                        FixedAssetReceiveValidationID = c.Int(nullable: false, identity: true),
                        FixedAssetReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        Approver = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReceiveValidationID)
                .ForeignKey("EXCEED.FixedAssetReceive", t => t.FixedAssetReceiveId, cascadeDelete: true)
                .Index(t => t.FixedAssetReceiveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetReceive", "ReceiveTypeId", "EXCEED.ReceiveType");
            DropForeignKey("EXCEED.FixedAssetInspection", "FixedAssetReceive_FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropForeignKey("EXCEED.FixedAssetReceiveValidation", "FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropForeignKey("EXCEED.FixedAssetReceiveDistribution", "FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropForeignKey("EXCEED.FixedAssetInspection", "GoodReceiveId", "EXCEED.GoodReceive");
            DropIndex("EXCEED.FixedAssetReceiveValidation", new[] { "FixedAssetReceiveId" });
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "ItemCode" });
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "FixedAssetReceiveId" });
            DropIndex("EXCEED.FixedAssetReceive", new[] { "ReceiveTypeId" });
            DropIndex("EXCEED.FixedAssetReceiveDistribution", new[] { "FixedAssetReceiveId" });
            DropIndex("EXCEED.FixedAssetInspection", new[] { "FixedAssetReceive_FixedAssetReceiveId" });
            DropIndex("EXCEED.FixedAssetInspection", new[] { "GoodReceiveId" });
            DropTable("EXCEED.FixedAssetReceiveValidation");
            DropTable("EXCEED.FixedAssetReceiveItem");
            DropTable("EXCEED.FixedAssetReceive");
            DropTable("EXCEED.FixedAssetReceiveDistribution");
            DropTable("EXCEED.FixedAssetInspection");
        }
    }
}
