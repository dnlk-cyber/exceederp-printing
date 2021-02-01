namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset9 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetCategoryID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetSubCategoryID" });
            AddColumn("EXCEED.FixedAssetIssueItem", "ItemName", c => c.String());
            AddColumn("EXCEED.FixedAssetReturnItem", "ItemName", c => c.String());
            CreateIndex("EXCEED.FixedAssetIssueItem", "AssetRegistrationId");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.FixedAssetReturnItem", "ItemName");
            DropColumn("EXCEED.FixedAssetIssueItem", "ItemName");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
        }
    }
}
