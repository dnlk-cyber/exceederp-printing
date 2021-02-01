namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assetrequi1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetIssue", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropIndex("EXCEED.FixedAssetIssue", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "AssetRegistrationId" });
            AddColumn("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "RequestedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "ApprovedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "IssuedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "PurchaseQuantity", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId");
            AddForeignKey("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition", "FixedAssetRequisitionId", cascadeDelete: true);
            DropColumn("EXCEED.FixedAssetIssue", "StoreRequisitionID");
            DropColumn("EXCEED.FixedAssetIssueItem", "GLAccountCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetIssueItem", "GLAccountCode", c => c.String());
            AddColumn("EXCEED.FixedAssetIssue", "StoreRequisitionID", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropIndex("EXCEED.FixedAssetIssue", new[] { "FixedAssetRequisitionId" });
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "PurchaseQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "IssuedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId");
            CreateIndex("EXCEED.FixedAssetIssueItem", "AssetRegistrationId");
            CreateIndex("EXCEED.FixedAssetIssue", "StoreRequisitionID");
            AddForeignKey("EXCEED.FixedAssetIssue", "StoreRequisitionID", "EXCEED.StoreRequisition", "StoreRequisitionID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
