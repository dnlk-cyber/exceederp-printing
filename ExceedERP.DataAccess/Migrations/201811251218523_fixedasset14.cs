namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset14 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetStoreReturnItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetStoreReturnItem", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn");
            DropForeignKey("EXCEED.FixedAssetStoreReturnValidation", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn");
            DropIndex("EXCEED.FixedAssetStoreReturnItem", new[] { "FixedAssetStoreReturnId" });
            DropIndex("EXCEED.FixedAssetStoreReturnItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetStoreReturnValidation", new[] { "FixedAssetStoreReturnId" });
            DropColumn("EXCEED.AssetTransfer", "FixedAssetCategoryId");
            DropColumn("EXCEED.AssetTransfer", "FixedAssetSubCategoryId");
            DropTable("EXCEED.FixedAssetStoreReturnItem");
            DropTable("EXCEED.FixedAssetStoreReturn");
            DropTable("EXCEED.FixedAssetStoreReturnValidation");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.FixedAssetStoreReturnValidationId);
            
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
                .PrimaryKey(t => t.FixedAssetStoreReturnItemId);
            
            AddColumn("EXCEED.AssetTransfer", "FixedAssetSubCategoryId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "FixedAssetCategoryId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetStoreReturnValidation", "FixedAssetStoreReturnId");
            CreateIndex("EXCEED.FixedAssetStoreReturnItem", "AssetRegistrationId");
            CreateIndex("EXCEED.FixedAssetStoreReturnItem", "FixedAssetStoreReturnId");
            AddForeignKey("EXCEED.FixedAssetStoreReturnValidation", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn", "FixedAssetStoreReturnId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetStoreReturnItem", "FixedAssetStoreReturnId", "EXCEED.FixedAssetStoreReturn", "FixedAssetStoreReturnId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetStoreReturnItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
