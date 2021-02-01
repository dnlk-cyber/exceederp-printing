namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetIssueDistribution",
                c => new
                    {
                        FixedAssetIssueDistributionID = c.Int(nullable: false, identity: true),
                        FixedAssetIssueID = c.Int(nullable: false),
                        FixedAssetIssueItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FixedAssetIssueDistributionID)
                .ForeignKey("EXCEED.FixedAssetIssue", t => t.FixedAssetIssueID, cascadeDelete: true)
                .Index(t => t.FixedAssetIssueID);
            
            CreateTable(
                "EXCEED.FixedAssetIssue",
                c => new
                    {
                        FixedAssetIssueID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        Reference = c.String(),
                        IssueDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.FixedAssetIssueID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.FixedAssetIssueItem",
                c => new
                    {
                        FixedAssetIssueItemID = c.Int(nullable: false, identity: true),
                        FixedAssetIssueID = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLAccountCode = c.String(),
                        TagNo = c.String(),
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetIssueItemID)
                .ForeignKey("EXCEED.FixedAssetIssue", t => t.FixedAssetIssueID, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.FixedAssetIssueID)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.FixedAssetIssueValidation",
                c => new
                    {
                        FixedAssetIssueValidationID = c.Int(nullable: false, identity: true),
                        FixedAssetIssueID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetIssueValidationID)
                .ForeignKey("EXCEED.FixedAssetIssue", t => t.FixedAssetIssueID, cascadeDelete: true)
                .Index(t => t.FixedAssetIssueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetIssue", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.FixedAssetIssueValidation", "FixedAssetIssueID", "EXCEED.FixedAssetIssue");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetIssueID", "EXCEED.FixedAssetIssue");
            DropForeignKey("EXCEED.FixedAssetIssueDistribution", "FixedAssetIssueID", "EXCEED.FixedAssetIssue");
            DropIndex("EXCEED.FixedAssetIssueValidation", new[] { "FixedAssetIssueID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "ItemCode" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetIssueID" });
            DropIndex("EXCEED.FixedAssetIssue", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.FixedAssetIssueDistribution", new[] { "FixedAssetIssueID" });
            DropTable("EXCEED.FixedAssetIssueValidation");
            DropTable("EXCEED.FixedAssetIssueItem");
            DropTable("EXCEED.FixedAssetIssue");
            DropTable("EXCEED.FixedAssetIssueDistribution");
        }
    }
}
