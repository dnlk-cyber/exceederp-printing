namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetReturnDistribution",
                c => new
                    {
                        FixedAssetReturnDistributionId = c.Int(nullable: false, identity: true),
                        FixedAssetReturnId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReturnDistributionId)
                .ForeignKey("EXCEED.FixedAssetReturn", t => t.FixedAssetReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetReturnId);
            
            CreateTable(
                "EXCEED.FixedAssetReturn",
                c => new
                    {
                        FixedAssetReturnId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        Reference = c.String(),
                        ReturnDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.FixedAssetReturnId);
            
            CreateTable(
                "EXCEED.FixedAssetReturnItem",
                c => new
                    {
                        FixedAssetReturnItemId = c.Int(nullable: false, identity: true),
                        FixedAssetReturnId = c.Int(nullable: false),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        TagNo = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReturnItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetReturn", t => t.FixedAssetReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetReturnId)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.FixedAssetReturnValidation",
                c => new
                    {
                        FixedAssetReturnValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetReturnId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReturnValidationId)
                .ForeignKey("EXCEED.FixedAssetReturn", t => t.FixedAssetReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetReturnId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetReturnValidation", "FixedAssetReturnId", "EXCEED.FixedAssetReturn");
            DropForeignKey("EXCEED.FixedAssetReturnItem", "FixedAssetReturnId", "EXCEED.FixedAssetReturn");
            DropForeignKey("EXCEED.FixedAssetReturnItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetReturnDistribution", "FixedAssetReturnId", "EXCEED.FixedAssetReturn");
            DropIndex("EXCEED.FixedAssetReturnValidation", new[] { "FixedAssetReturnId" });
            DropIndex("EXCEED.FixedAssetReturnItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetReturnItem", new[] { "FixedAssetReturnId" });
            DropIndex("EXCEED.FixedAssetReturnDistribution", new[] { "FixedAssetReturnId" });
            DropTable("EXCEED.FixedAssetReturnValidation");
            DropTable("EXCEED.FixedAssetReturnItem");
            DropTable("EXCEED.FixedAssetReturn");
            DropTable("EXCEED.FixedAssetReturnDistribution");
        }
    }
}
