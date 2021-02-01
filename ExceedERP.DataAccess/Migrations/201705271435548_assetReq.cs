namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assetReq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetRequisitionItem",
                c => new
                    {
                        FixedAssetRequisitionItemId = c.Int(nullable: false, identity: true),
                        FixedAssetRequisitionId = c.Int(nullable: false),
                        CategoryCode = c.String(nullable: false),
                        SubCategoryCode = c.String(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        GeneratePurchaseReq = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        FixedAssetCategory_FixedAssetCategoryID = c.Int(),
                        FixedAssetSubCategory_FixedAssetSubCategoryID = c.Int(),
                    })
                .PrimaryKey(t => t.FixedAssetRequisitionItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetCategory", t => t.FixedAssetCategory_FixedAssetCategoryID)
                .ForeignKey("EXCEED.FixedAssetRequisition", t => t.FixedAssetRequisitionId, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetSubCategory", t => t.FixedAssetSubCategory_FixedAssetSubCategoryID)
                .Index(t => t.FixedAssetRequisitionId)
                .Index(t => t.AssetRegistrationId)
                .Index(t => t.FixedAssetCategory_FixedAssetCategoryID)
                .Index(t => t.FixedAssetSubCategory_FixedAssetSubCategoryID);
            
            CreateTable(
                "EXCEED.FixedAssetRequisition",
                c => new
                    {
                        FixedAssetRequisitionId = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        StoreCode = c.String(nullable: false),
                        ToStoreCode = c.String(),
                        IsFuel = c.Boolean(nullable: false),
                        IsTransfer = c.Boolean(nullable: false),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.FixedAssetRequisitionId);
            
            CreateTable(
                "EXCEED.FixedAssetRequisitionPropertyValidation",
                c => new
                    {
                        FixedAssetRequisitionPropertyValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetRequisitionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        Approver = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetRequisitionPropertyValidationId)
                .ForeignKey("EXCEED.FixedAssetRequisition", t => t.FixedAssetRequisitionId, cascadeDelete: true)
                .Index(t => t.FixedAssetRequisitionId);
            
            CreateTable(
                "EXCEED.FixedAssetRequisitionValidation",
                c => new
                    {
                        FixedAssetRequisitionValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetRequisitionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetRequisitionValidationId)
                .ForeignKey("EXCEED.FixedAssetRequisition", t => t.FixedAssetRequisitionId, cascadeDelete: true)
                .Index(t => t.FixedAssetRequisitionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.FixedAssetRequisitionValidation", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropForeignKey("EXCEED.FixedAssetRequisitionPropertyValidation", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetRequisitionValidation", new[] { "FixedAssetRequisitionId" });
            DropIndex("EXCEED.FixedAssetRequisitionPropertyValidation", new[] { "FixedAssetRequisitionId" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetRequisitionId" });
            DropTable("EXCEED.FixedAssetRequisitionValidation");
            DropTable("EXCEED.FixedAssetRequisitionPropertyValidation");
            DropTable("EXCEED.FixedAssetRequisition");
            DropTable("EXCEED.FixedAssetRequisitionItem");
        }
    }
}
