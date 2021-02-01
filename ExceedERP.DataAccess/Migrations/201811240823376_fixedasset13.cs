namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset13 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AssetTransferItem",
                c => new
                    {
                        AssetTransferItemId = c.Int(nullable: false, identity: true),
                        AssetTransferId = c.Int(nullable: false),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        Size = c.String(),
                        ModelNo = c.String(),
                        SerialNo = c.String(),
                        TagNo = c.String(),
                        Quantity = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetTransferItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.AssetTransfer", t => t.AssetTransferId, cascadeDelete: true)
                .Index(t => t.AssetTransferId)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.AssetTransfer",
                c => new
                    {
                        AssetTransferId = c.Int(nullable: false, identity: true),
                        RequestNumber = c.String(),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        TransferDate = c.DateTime(nullable: false),
                        TransferDesc = c.String(),
                        CustodianBeforeTransfer = c.Int(nullable: false),
                        TransferTo = c.Int(nullable: false),
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
                .PrimaryKey(t => t.AssetTransferId);
            
            CreateTable(
                "EXCEED.AssetTransferValidation",
                c => new
                    {
                        AssetTransferValidationId = c.Int(nullable: false, identity: true),
                        AssetTransferId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetTransferValidationId)
                .ForeignKey("EXCEED.AssetTransfer", t => t.AssetTransferId, cascadeDelete: true)
                .Index(t => t.AssetTransferId);
            
            CreateTable(
                "EXCEED.FixedAssetCountItem",
                c => new
                    {
                        FixedAssetCountItemId = c.Int(nullable: false, identity: true),
                        FixedAssetCountId = c.Int(nullable: false),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        PreviousBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Count = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Boolean(nullable: false),
                        Updated = c.Boolean(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Location = c.String(),
                        ItemName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ShortageQty = c.Decimal(precision: 18, scale: 2),
                        OverageQty = c.Decimal(precision: 18, scale: 2),
                        ShortagePrice = c.Decimal(precision: 18, scale: 2),
                        OveragePrice = c.Decimal(precision: 18, scale: 2),
                        Amouont = c.Decimal(precision: 18, scale: 2),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        PhysicalAmount = c.Decimal(precision: 18, scale: 2),
                        Uom = c.String(),
                        ItemDescription = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.FixedAssetCountItemId)
                .ForeignKey("EXCEED.FixedAssetCount", t => t.FixedAssetCountId, cascadeDelete: true)
                .Index(t => t.FixedAssetCountId);
            
            CreateTable(
                "EXCEED.FixedAssetCount",
                c => new
                    {
                        FixedAssetCountId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        StoreCode = c.String(maxLength: 128),
                        CountDate = c.DateTime(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        IsMidOfPeriod = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.FixedAssetCountId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode)
                .Index(t => t.BranchId)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.FixedAssetCountValidation",
                c => new
                    {
                        FixedAssetCountValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetCountId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetCountValidationId)
                .ForeignKey("EXCEED.FixedAssetCount", t => t.FixedAssetCountId, cascadeDelete: true)
                .Index(t => t.FixedAssetCountId);
            
            CreateTable(
                "EXCEED.FixedAssetStoreReturnItem",
                c => new
                    {
                        FixedAssetStoreReturnItemId = c.Int(nullable: false, identity: true),
                        FixedAssetStoreReturnId = c.Int(nullable: false),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        Size = c.String(),
                        ModelNo = c.String(),
                        SerialNo = c.String(),
                        TagNo = c.String(),
                        Quantity = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetStoreReturnItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetStoreReturn", t => t.FixedAssetStoreReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetStoreReturnId)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.FixedAssetStoreReturn",
                c => new
                    {
                        FixedAssetStoreReturnId = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        StoreCode = c.String(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.FixedAssetStoreReturnId);
            
            CreateTable(
                "EXCEED.FixedAssetStoreReturnValidation",
                c => new
                    {
                        FixedAssetStoreReturnValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetStoreReturnId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetStoreReturnValidationId)
                .ForeignKey("EXCEED.FixedAssetStoreReturn", t => t.FixedAssetStoreReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetStoreReturnId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetStoreReturnValidation", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn");
            DropForeignKey("EXCEED.FixedAssetStoreReturnItem", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn");
            DropForeignKey("EXCEED.FixedAssetStoreReturnItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetCountValidation", "FixedAssetCountId", "EXCEED.FixedAssetCount");
            DropForeignKey("EXCEED.FixedAssetCount", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.FixedAssetCountItem", "FixedAssetCountId", "EXCEED.FixedAssetCount");
            DropForeignKey("EXCEED.FixedAssetCount", "BranchId", "HRM.Branch");
            DropForeignKey("EXCEED.AssetTransferValidation", "AssetTransferId", "EXCEED.AssetTransfer");
            DropForeignKey("EXCEED.AssetTransferItem", "AssetTransferId", "EXCEED.AssetTransfer");
            DropForeignKey("EXCEED.AssetTransferItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetStoreReturnValidation", new[] { "FixedAssetStoreReturnId" });
            DropIndex("EXCEED.FixedAssetStoreReturnItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetStoreReturnItem", new[] { "FixedAssetStoreReturnId" });
            DropIndex("EXCEED.FixedAssetCountValidation", new[] { "FixedAssetCountId" });
            DropIndex("EXCEED.FixedAssetCount", new[] { "StoreCode" });
            DropIndex("EXCEED.FixedAssetCount", new[] { "BranchId" });
            DropIndex("EXCEED.FixedAssetCountItem", new[] { "FixedAssetCountId" });
            DropIndex("EXCEED.AssetTransferValidation", new[] { "AssetTransferId" });
            DropIndex("EXCEED.AssetTransferItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.AssetTransferItem", new[] { "AssetTransferId" });
            DropTable("EXCEED.FixedAssetStoreReturnValidation");
            DropTable("EXCEED.FixedAssetStoreReturn");
            DropTable("EXCEED.FixedAssetStoreReturnItem");
            DropTable("EXCEED.FixedAssetCountValidation");
            DropTable("EXCEED.FixedAssetCount");
            DropTable("EXCEED.FixedAssetCountItem");
            DropTable("EXCEED.AssetTransferValidation");
            DropTable("EXCEED.AssetTransfer");
            DropTable("EXCEED.AssetTransferItem");
        }
    }
}
